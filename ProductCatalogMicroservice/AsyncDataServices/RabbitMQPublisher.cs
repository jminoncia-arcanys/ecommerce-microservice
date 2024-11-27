using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace ProductCatalogMicroservice.AsyncDataServices
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly string _hostname = "localhost"; // RabbitMQ host name
        private readonly string _exchange = "event_bus_exchange"; // Exchange name

        public async Task PublishAsync(object eventData)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
               
                channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);
                var message = JsonSerializer.Serialize(eventData);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: _exchange, routingKey: "", basicProperties: null, body: body);
            }

            await Task.CompletedTask;
        }
    }
}
