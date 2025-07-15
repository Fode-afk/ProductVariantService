namespace ProductService.AsyncDataServices
{
    public class MessageBusClientInitializer(IMessageBusClient messageBusClient) : IHostedService
    {
        private readonly IMessageBusClient _messageBusClient = messageBusClient;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBusClient.InitAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
