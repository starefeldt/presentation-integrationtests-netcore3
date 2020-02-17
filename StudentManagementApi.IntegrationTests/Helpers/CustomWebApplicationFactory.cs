using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentManagementApi.DataAccess;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using StudentManagementApi.IntegrationTests.Repositories;
using System;
using System.Linq;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private DockerSqlHelper _dockerHelper;

        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder()
                .UseEnvironment(AppEnvironment.IntegrationTesting);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Always use IServiceScope to resolve instances when getting services from the
            // IOC-container (memory-leeks may occur otherwise).
            builder.ConfigureServices(services =>
            {
                if (services.HasRegistered<IStudentRepository, StudentRepositoryEF>())
                {
                    WhenRegisteredEntityFramework(services);
                }
                else if (services.HasRegistered<IStudentRepository, StudentRepositorySQL>())
                {
                    WhenRegisteredSqlConnection(services);
                }
                else
                {
                    throw new InvalidOperationException($"Missing registration in {nameof(IServiceCollection)} for: {nameof(IStudentRepository)}");
                }

                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var testRepo = scopedServices.GetRequiredService<IDbTestRepository>();
                    _dockerHelper.WriteDebugMessage("Creating database...");
                    testRepo.CreateDatabase();
                    _dockerHelper.WriteDebugMessage("Database created");
                }

                services.RemoveDescriptorFor<IMessageSender>();
            });
        }

        private void WhenRegisteredEntityFramework(IServiceCollection services)
        {
            services.AddDbContext<StudentManagementDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            // Seed database with data here or let each test handle it.
            // The actual http-request scoped instance of DbContext can't be obtained from the test.
            // However, will work with a new DbContext since it uses the same DbContextOptions.
            services.AddScoped<IDbTestRepository, TestRepositoryEF>();
        }

        private void WhenRegisteredSqlConnection(IServiceCollection services)
        {
            // Spin up Docker Container
            // Register DbConnectionFactory with connectionstring for Docker container
            try
            {
                if (_dockerHelper == null)
                {
                    var containerName = GetUniqueContainerName();
                    var config = new DockerConfiguration
                    {
                        ContainerName = containerName,
                        Port = PortDistributor.GetPortFor(containerName)
                    };

                    var dbConnectionFactory = GetDbConnectionFactory(services, config.Port);
                    _dockerHelper = new DockerSqlHelper(config, dbConnectionFactory);
                    _dockerHelper.StartContainer();
                }
                services.AddScoped(_ => _dockerHelper.DbConnectionFactory);
                services.AddScoped<IDbTestRepository, TestRepositorySQL>();
            }
            catch (Exception ex)
            {
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                    logger.LogError(ex.ToString());
                    throw;
                }
            }
        }

        private string GetUniqueContainerName() =>
            Guid
            .NewGuid()
            .ToString()
            .Replace("-", "")
            .Substring(0, 8);

        private DbConnectionFactory GetDbConnectionFactory(IServiceCollection services, int port)
        {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var configuration = scopedServices.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("IntegrationTestingDb");
                connectionString = connectionString.Replace("[port]", port.ToString());
                return new DbConnectionFactory(connectionString);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_dockerHelper != null)
            {
                _dockerHelper.RemoveContainer();
            }
            base.Dispose(disposing);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void RemoveDescriptorFor<TService>(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(TService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

        public static bool HasRegistered<TService, TImplementation>(this IServiceCollection services)
        {
            return services
                .Where(s => s.ServiceType == typeof(TService))
                .Any(m => m.ImplementationType == typeof(TImplementation));
        }
    }
}