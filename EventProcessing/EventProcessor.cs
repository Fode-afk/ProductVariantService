using AutoMapper;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.Data.Caching;
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
                case EventType.ImageDeletePublished:
                    await HandleEventAsync<List<ImagePublishedDto>>(message, DeleteProductImages);
                    break;
                case EventType.CardDeletePublished:
                    await HandleEventAsync<CardPublishedDto>(message, DeleteParentCardIdFromProducts);
                    break;
                case EventType.VendorDeletePublished:
                    await HandleEventAsync<VendorPublishedDto>(message, DeleteProductsByVendorId);
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
            var cacheRepo = scope.ServiceProvider.GetRequiredService<ICacheRepo>();

            var product = new Product
            {
                ProductId = publishedDto.Id,
                ImageURLs = [publishedDto.Url],
                UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(_config)
            };

            var res = await repo.AddImagesToProductAsync(product);

            if (res.success)
            {
                await cacheRepo.RemoveAsync($"product:{product.ProductId}");

                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);
            }
        }

        private async Task DeleteProductImages(List<ImagePublishedDto> publishedDtos)
        {
            if (publishedDtos.Any(dto => dto.ContentType != ContentType.PRODUCT_IMAGE))
                await Task.FromException(new Exception("ContentType doesn't match"));

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
            var cacheRepo = scope.ServiceProvider.GetRequiredService<ICacheRepo>();

            var product = new Product
            {
                ProductId = publishedDtos[0].Id,
                ImageURLs = [..publishedDtos.Select(dto => dto.Url)],
                UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(_config)
            };

            var res = await repo.DeleteImagesFromProductAsync(product);

            if (res.success)
            {
                await cacheRepo.RemoveAsync($"product:{product.ProductId}");

                var productPub = _mapper.Map<ProductPublishedDto>(product);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);
            }
        }


        private async Task DeleteParentCardIdFromProducts(CardPublishedDto publishedDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
            var cacheRepo = scope.ServiceProvider.GetRequiredService<ICacheRepo>();

            var res = await repo.DeleteParentCardIdFromProductsAsync([.. publishedDto.ProductIds],
                publishedDto.CardId, DateTimeUtil.GetCurrentTimeFormatted(_config));

            if (res.success)
            {
                foreach (var productId in res.Value.Select(p => p.ProductId))
                    await cacheRepo.RemoveAsync($"product:{productId}");

                var productsPub = _mapper.Map<List<ProductPublishedDto>>(res.Value);

                await _messageBusClient.PublishGenericEvent(productsPub, EventType.DeleteParentCardIdFromProducts, [ServicesEnum.SEARCH_SERVICE]);
            }
        }

        private async Task DeleteProductsByVendorId(VendorPublishedDto publishedDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
            var cacheRepo = scope.ServiceProvider.GetRequiredService<ICacheRepo>();

            var res = await productRepo.DeleteProductsByOwnerIdAsync(publishedDto.VendorId);

            if (res.success)
            {
                foreach (var product in res.Value)
                {
                    string cacheKey = $"product:{product.ProductId}";
                    await cacheRepo.RemoveAsync(cacheKey);
                }

                var productsPub = _mapper.Map<List<ProductPublishedDto>>(res.Value);

                await _messageBusClient.PublishGenericEvent(productsPub, EventType.ProductsDeletePublished,
                    [ServicesEnum.SEARCH_SERVICE, ServicesEnum.REVIEW_SERVICE]);

                List<ImagePublishedDto> publishedDtos = [];

                foreach (var product in res.Value)
                {
                    if (res.Value.Any(p => p.ImageURLs.Count > 0))
                    {                      
                        foreach (var url in product.ImageURLs)
                            publishedDtos.Add(new ImagePublishedDto { Id = product.ProductId, ContentType = ContentType.PRODUCT_IMAGE, Url = url });
                    }
                }

                await _messageBusClient.PublishGenericEvent(publishedDtos, EventType.ImageDeletePublished, [ServicesEnum.IMAGE_SERVICE]);
            }
        }
    }
}
