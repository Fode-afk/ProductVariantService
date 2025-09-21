using migApp.Shared.Enums;
using migApp.Shared.EventDtos;
using migApp.Shared.MsgBus;
using migApp.Shared.MsgBus.Dtos;
using migApp.Shared.MsgBus.Dtos.Images;
using migApp.Shared.MsgBus.Dtos.Product;
using migApp.Shared.MsgBus.Enums;
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
        private string _exchange;
        private Dictionary<ServicesEnum, string> _routingMap;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public async Task InitAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"],
                Port = int.Parse(_config["RabbitMQ:Port"])
            };

            _exchange = _config["RabbitMQ:Exchange"];

            _routingMap = Enum.GetValues<ServicesEnum>()
                .ToDictionary(
                    key => key,
                    key => _config[$"RabbitMQ:RoutingKeys:{key}"]
                )!;

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: _exchange,
                    type: ExchangeType.Direct,
                    durable: true
                );

                foreach (var (service, routingKey) in _routingMap)
                {
                    string queueName = $"{routingKey}-queue";

                    await _channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false
                    );

                    await _channel.QueueBindAsync(
                        queue: queueName,
                        exchange: _exchange,
                        routingKey: routingKey
                    );
                }

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

        public async Task PublishEventAsync<TPayload, TEventEnum>(TPayload payload, TEventEnum eventType, ServicesEnum[] consumers)
                where TEventEnum : Enum
        {
            BaseEventDto eventDto = eventType switch
            {
                ProductEvents => new ProductEventDto<TPayload>
                {
                    Category = EventCategory.Product,
                    Consumers = consumers,
                    EventType = (ProductEvents)(object)eventType,
                    Data = payload
                },

                ImageEvents => new ImageEventDto<TPayload>
                {
                    Category = EventCategory.Image,
                    Consumers = consumers,
                    EventType = (ImageEvents)(object)eventType,
                    Data = payload
                },

                _ => throw new NotSupportedException($"Unknown event enum: {eventType}")
            };

            var message = JsonSerializer.Serialize(eventDto, jsonSerializerOptions);

            if (_connection?.IsOpen == true)
            {
                Console.WriteLine($"--> Sending {eventDto.Category} event ({eventType})...");
                foreach (var service in consumers)
                {
                    if (_routingMap.TryGetValue(service, out var routingKey))
                    {
                        await SendMessageAsync(message, routingKey);
                    }
                    else
                    {
                        Console.WriteLine($"--> No routing key found for service: {service}");
                    }
                }
            }
        }

        private async Task SendMessageAsync(string message, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body
            );

            Console.WriteLine($"--> Message sent to {routingKey}: {message}");
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
