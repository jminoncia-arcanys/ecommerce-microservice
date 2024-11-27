using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;

namespace UserAuthMicroservice.AsyncDataServices
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly string _hostname = "localhost"; 
        private readonly string _queue = "cart_events_queue";

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var eventData = JsonSerializer.Deserialize<dynamic>(message);

                    if (eventData?.Action == "AddToCart")
                    {
                        await HandleAddToCartEvent(eventData.UserId, eventData.ProductId, eventData.Quantity);
                    }
                };

                channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);

                return Task.CompletedTask;
            }
        }

        private async Task HandleAddToCartEvent(int userId, int productId, int quantity)
        {
            Console.WriteLine($"Received AddToCart event: UserId={userId}, ProductId={productId}, Quantity={quantity}");

            await Task.CompletedTask;
        }
    }
}
