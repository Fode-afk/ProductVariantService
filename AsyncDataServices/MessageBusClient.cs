using ProductService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProductService.AsyncDataServices
{
    public class MessageBusClient(IConfiguration configuration) : IMessageBusClient
    {
        private readonly IConfiguration _config = configuration;
        private IConnection _connection;
        private IChannel _channel;

        public async Task InitAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout,
                    durable: true
                );

                string queueName = "productService-queue";
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: "trigger",
                    routingKey: ""
                );

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the message bus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }
        }

        private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown.");
            return Task.CompletedTask;
        }

        public async Task PublishNewProduct(ProductPublishedDto productPublishedDto)
        {
            var message = JsonSerializer.Serialize(productPublishedDto);

            if (_connection?.IsOpen == true)
            {
                Console.WriteLine("--> RabbitMQ connection is open, sending message...");
                await SendMessageAsync(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed. Message not sent.");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "trigger",
                routingKey: "",
                mandatory: true,
                basicProperties: new BasicProperties() { Persistent = true },
                body: body);

            Console.WriteLine($"--> Message sent: {message}");
        }

        public async ValueTask Dispose()
        {
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }

            Console.WriteLine("--> Message bus client disposed.");
        }
    }
}
