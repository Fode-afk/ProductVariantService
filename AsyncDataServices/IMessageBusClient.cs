using migApp.Shared.Enums;

namespace ProductService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task InitAsync();
        Task PublishGenericEvent<T>(T payload, EventType eventType, ServicesEnum[] consumers);
        ValueTask Dispose();
    }
}
