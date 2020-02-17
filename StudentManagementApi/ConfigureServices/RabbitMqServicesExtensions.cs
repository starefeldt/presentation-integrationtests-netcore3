using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StudentManagementApi.Domain;
using StudentManagementApi.Domain.Configuration;
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
}
