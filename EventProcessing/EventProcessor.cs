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

            var imagePublishedDto = JsonSerializer.Deserialize<ImagePublishedDto>(message);

            if(imagePublishedDto.Service != ServicesEnum.PRODUCT_SERVICE)
            {
                Console.WriteLine($"--> Ignoring event for service: {imagePublishedDto.Service}");
                return;
            }


            switch (imagePublishedDto.Event)
            {
                case EventType.ImageUrlPublished:
                    await UpdateProduct(imagePublishedDto);
                    break;
                default:
                    break;
            }

            return;
        }

        private async Task UpdateProduct(ImagePublishedDto publishedDto)
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
