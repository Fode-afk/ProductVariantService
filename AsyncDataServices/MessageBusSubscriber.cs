using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProductService.AsyncDataServices
{
    public class MessageBusSubscriber(IConfiguration configuration) : BackgroundService
    {
        private readonly IConfiguration _config = configuration;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitRabbitMQAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"--> Event received: {message}");
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            Console.WriteLine("--> Listening for messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task InitRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

            var queueDeclareOk = await _channel.QueueDeclareAsync();
            _queueName = queueDeclareOk.QueueName;

            await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> RabbitMQ connected and queue bound.");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }

        private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            return Task.CompletedTask;
        }

        public async ValueTask Dispose()
        {
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }
    }
}
