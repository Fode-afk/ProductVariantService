using ProductService.Data;
using ProductService.Dtos;
using ProductService.Models;
using System.Text.Json;

namespace ProductService.EventProcessing
{
    public class EventProcessor(IServiceScopeFactory scopeFactory) : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task ProcessEventAsync(string message)
        {          
            GenericEventDto<object>? generic;
            try
            {
                generic = JsonSerializer.Deserialize<GenericEventDto<object>>(message, jsonSerializerOptions);
                if (generic == null)
                {
                    Console.WriteLine("--> Failed to parse GenericEventDto");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Deserialization error: {ex.Message}");
                return;
            }

            if (!generic.Consumers.Contains(ServicesEnum.PRODUCT_SERVICE))
            {
                Console.WriteLine($"--> Ignoring event not for PRODUCT_SERVICE (for: {string.Join(", ", generic.Consumers)})");
                return;
            }

            switch (generic.EventType)
            {
                case EventType.ImageUrlPublished:
                    await HandleEventAsync<ImagePublishedDto>(message, AddProductImage);
                    break;
                default:
                    Console.WriteLine($"--> Unknown or unhandled event type: {generic.EventType}");
                    break;
            }
        }

        private async Task HandleEventAsync<T>(string message, Func<T, Task> handler)
        {
            try
            {
                var typedEvent = JsonSerializer.Deserialize<GenericEventDto<T>>(message, jsonSerializerOptions);
                if (typedEvent.Data is not null)
                {
                    await handler(typedEvent.Data);
                }
                else
                {
                    Console.WriteLine("--> Typed event data was null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Deserialization error (typed): {ex.Message}");
            }
        }

        private async Task AddProductImage(ImagePublishedDto publishedDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();

            var product = new Product
            {
                ProductId = publishedDto.Id,
                ImageURLs = [publishedDto.Url]
            };

            await repo.AddImagesToProductAsync(product);
        }
    }
}
