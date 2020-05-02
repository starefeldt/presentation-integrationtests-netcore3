using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManagementApi.DataAccess;
using StudentManagementApi.Domain.Configuration;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using StudentManagementApi.IntegrationTests.Helpers;
using StudentManagementApi.IntegrationTests.Repositories;
using System;
using System.Diagnostics;
using System.Linq;

namespace StudentManagementApi.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private DockerSqlHelper _dockerHelper;

        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder()
                .UseEnvironment(AppEnvironment.IntegrationTesting);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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

                // Always use IServiceScope to resolve instances when getting services from the
                // IOC-container (memory-leeks may occur otherwise).
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var testRepo = scopedServices.GetRequiredService<IDbTestRepository>();
                    Debug.WriteLine("Creating database...");
                    testRepo.CreateDatabase();
                    Debug.WriteLine("Database created");
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
            // The actual http-request scoped instance of DbContext can't be obtained from within the test.
            // However, it will work with a new DbContext since it uses the same DbContextOptions.
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
                    var port = PortDistributor.GetPortFor(containerName);
                    var config = new DockerConfiguration
                    {
                        ContainerName = containerName,
                        Port = port,
                        SaPassword = GetSaPassword(services)
                    };

                    var dbConnectionFactory = GetDbConnectionFactory(services, port);
                    _dockerHelper = new DockerSqlHelper(config, dbConnectionFactory);
                    _dockerHelper.StartContainer();
                }
                services.AddScoped(_ => _dockerHelper.DbConnectionFactory);
                services.AddScoped<IDbTestRepository, TestRepositorySQL>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private string GetUniqueContainerName() =>
            Guid
            .NewGuid()
            .ToString()
            .Replace("-", "")
            .Substring(0, 8);

        private string GetSaPassword(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var configuration = scopedServices.GetRequiredService<IConfiguration>();
                return configuration.GetSection("SqlCredentials:SaPassword").Value;
            }
        }

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