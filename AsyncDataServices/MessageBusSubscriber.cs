using migApp.Shared.MsgBus;
using ProductService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProductService.AsyncDataServices
{
    public class MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService
    {
        private readonly IConfiguration _config = configuration;
        private readonly IEventProcessor _eventProcessor = eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string _queueName = configuration["RabbitMQ:RoutingKeys:PRODUCT_SERVICE"] + "-queue";
        private readonly string _exchangeName = configuration["RabbitMQ:Exchange"];
        private readonly string _routingKey = configuration["RabbitMQ:RoutingKeys:PRODUCT_SERVICE"];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitRabbitMQAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"--> Event received: {message}");
                await _eventProcessor.ProcessEventAsync(message);

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            Console.WriteLine($"--> Listening for messages on queue: {_queueName}");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task InitRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"],
                Port = int.Parse(_config["RabbitMQ:Port"])
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
           
            await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _routingKey
            );

            Console.WriteLine($"--> Queue '{_queueName}' bound to exchange '{_exchangeName}' with routing key '{_routingKey}'");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }

        private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shut down");
            return Task.CompletedTask;
        }

        public async ValueTask Dispose()
        {
            if (_channel?.IsOpen == true)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }
    }
}
