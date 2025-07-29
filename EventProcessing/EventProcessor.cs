using AutoMapper;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.Dtos;
using ProductService.Models;
using ProductService.Utils;
using System.Text.Json;

namespace ProductService.EventProcessing
{
    public class EventProcessor(IServiceScopeFactory scopeFactory, IConfiguration config,
        IMapper mapper, IMessageBusClient messageBusClient) : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IConfiguration _config = config;
        private readonly IMapper _mapper = mapper;
        private readonly IMessageBusClient _messageBusClient = messageBusClient;
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
                case EventType.CardDeletePublished:
                    await HandleEventAsync<CardPublishedDto>(message, DeleteParentCardIdFromProducts);
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
            if (publishedDto.ContentType != ContentType.PRODUCT_IMAGE)
                await Task.FromException(new Exception("ContentType doesn't match"));

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();

            var product = new Product
            {
                ProductId = publishedDto.Id,
                ImageURLs = [publishedDto.Url],
                UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(_config)
            };

            await repo.AddImagesToProductAsync(product);
        }

        private async Task DeleteParentCardIdFromProducts(CardPublishedDto publishedDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();

            var res = await repo.DeleteParentCardIdFromProductsAsync([.. publishedDto.ProductIds],
                publishedDto.CardId, DateTimeUtil.GetCurrentTimeFormatted(_config));

            if (res.success)
            {
                var productsPub = _mapper.Map<List<ProductPublishedDto>>(res.Value);

                await _messageBusClient.PublishGenericEvent(productsPub, EventType.DeleteParentCardIdFromProducts, [ServicesEnum.SEARCH_SERVICE]);
            }
        }
    }
}
