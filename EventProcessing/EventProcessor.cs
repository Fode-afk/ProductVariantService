using ProductService.Data;
using ProductService.Dtos;
using ProductService.Models;
using System.Text.Json;

namespace ProductService.EventProcessing
{
    public class EventProcessor(IServiceScopeFactory scopeFactory) : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task ProcessEventAsync(string message)
        {

            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.ImageUrlPublished:
                    await UpdateProduct(message);
                    break;
                default:
                    break;
            }

            return;
        }
        private EventType DetermineEvent(string notifcationMessage)
        {
            Console.WriteLine("--> Determining Event...");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifcationMessage);

            switch (eventType?.Event)
            {
                case "ImageUrlPublished":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.ImageUrlPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private async Task UpdateProduct(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
            var imagePublishedDto = JsonSerializer.Deserialize<ImagePublishedDto>(platformPublishedMessage);

            var product = new Product
            {
                ProductId = imagePublishedDto.ProductId,
                ImageURLs = [imagePublishedDto.Url]
            };

            await repo.UpdateProductAsync(product);
        }
    }
}
