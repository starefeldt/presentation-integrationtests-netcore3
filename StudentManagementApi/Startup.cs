using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManagementApi.ConfigureServices;
using StudentManagementApi.DataAccess;
using StudentManagementApi.Domain;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;

namespace StudentManagementApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesNotIntendedForIntegationTests(services);

            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStudentRepository, StudentRepositorySQL>();

            services.AddRabbitMQ(Configuration);
            services.AddControllers();
        }

        private void ConfigureServicesNotIntendedForIntegationTests(IServiceCollection services)
        {
            if (Env.EnvironmentName == AppEnvironment.IntegrationTesting)
            {
                return;
            }
            var connectionString = Configuration.GetConnectionString("StudentManagementDb");

            services.AddDbContext<StudentManagementDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("StudentManagementApi"));
            });

            services.AddScoped<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString));
            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureNotIntendedForIntegrationTests(app);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureNotIntendedForIntegrationTests(IApplicationBuilder app)
        {
            if (Env.EnvironmentName == AppEnvironment.IntegrationTesting)
            {
                return;
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.GetSwaggerUIOptions());
        }
    }
}
