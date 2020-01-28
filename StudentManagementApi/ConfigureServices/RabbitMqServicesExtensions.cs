using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StudentManagementApi.Domain;
using StudentManagementApi.Domain.Interfaces;

namespace StudentManagementApi.ConfigureServices
{
    public static class RabbitMqServicesExtensions
    {
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitConfig = configuration.GetSection("RabbitCredentials").Get<RabbitConfiguration>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitConfig.Host,
                VirtualHost = rabbitConfig.VirtualHost,
                Port = rabbitConfig.Port,
                UserName = rabbitConfig.User,
                Password = rabbitConfig.Password
            };
            services.AddSingleton<IMessageSender>(new RabbitMQSender(factory));
        }
    }

    public class RabbitConfiguration : IRabbitConfiguration
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
