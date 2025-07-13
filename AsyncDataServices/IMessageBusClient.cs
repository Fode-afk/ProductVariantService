using ProductService.Dtos;

namespace ProductService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task InitAsync();
        Task PublishNewProduct(ProductPublishedDto productPublishedDto);
        ValueTask Dispose();
    }
}
