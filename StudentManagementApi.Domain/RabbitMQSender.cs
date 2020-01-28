using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System.Text;

namespace StudentManagementApi.Domain
{
    public class RabbitMQSender : IMessageSender
    {
        private readonly IConnectionFactory _factory;

        public RabbitMQSender(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public bool Send(Message message)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ConfirmSelect();
                
                var jsonMessage = JsonConvert.SerializeObject(message, new StringEnumConverter());
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "TestPublish",
                    body: Encoding.UTF8.GetBytes(jsonMessage));
                
                return channel.WaitForConfirms();
            }
        }
    }
}
