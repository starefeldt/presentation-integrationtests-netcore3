using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManagementApi.DataAccess;
using StudentManagementApi.Domain.Interfaces;
using System.Linq;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder()
                .UseEnvironment(AppEnvironment.IntegrationTesting);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveDescriptorFor<StudentManagementDbContext>();
                services.AddDbContext<StudentManagementDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.RemoveDescriptorFor<IMessageSender>();

                // Seed database with data here in ConfigureServices or let each test handle it.
                // The actual http-request scoped instance of DbContext can't be obtained from the test.
                // However, will work with a new DbContext since it uses the same DbContextOptions.
                // Always use IServiceScope to resolve instances when getting services from the
                // IOC-container (memory-leeks may occur otherwise).

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<StudentManagementDbContext>();
                    context.Database.EnsureCreated();
                    //Seed db here if appropriate
                }
            });
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static void RemoveDescriptorFor<T>(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(T));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }
    }
}