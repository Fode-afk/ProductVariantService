using AutoMapper;
using migApp.Shared.Enums.Image;
using migApp.Shared.EventDtos;
using migApp.Shared.MsgBus;
using migApp.Shared.MsgBus.Dtos;
using migApp.Shared.MsgBus.Dtos.Images;
using migApp.Shared.MsgBus.Dtos.Product;
using migApp.Shared.MsgBus.Enums;
using ProductService.Data;
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
            BaseEventDto baseEvent;
            try
            {
                baseEvent = JsonSerializer.Deserialize<BaseEventDto>(message, jsonSerializerOptions);
                if (baseEvent == null)
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

            if (!baseEvent.Consumers.Contains(ServicesEnum.PRODUCT_SERVICE))
            {
                Console.WriteLine($"--> Ignoring event not for PRODUCT_SERVICE (for: {string.Join(", ", baseEvent.Consumers)})");
                return;
            }

            switch (baseEvent.Category)
            {
                case EventCategory.Image:
                    await HandleImageEvent(message);
                    break;

                default:
                    Console.WriteLine($"--> Unknown event category: {baseEvent.Category}");
                    break;
            }
        }

        private async Task HandleImageEvent(string message)
        {
            ImageEventDto<ImageDtoClass>? imageEvent;
            try
            {
                imageEvent = JsonSerializer.Deserialize<ImageEventDto<ImageDtoClass>>(message, jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Product event deserialization failed: {ex.Message}");
                return;
            }

            if (imageEvent == null)
                return;

            switch (imageEvent.EventType)
            {
                case ImageEvents.Published:
                    var published = (ImagePublishedDto)imageEvent.Data;
                    await AddProductImage(published);
                    break;

                case ImageEvents.Deleted:
                    //var updated = (ProductUpdatePublishedDto)productEvent.Data;
                    //await UpdateProduct(updated);
                    break;

                default:
                    Console.WriteLine($"--> Unhandled product event type: {imageEvent.EventType}");
                    break;
            }
        }

        private async Task AddProductImage(ImagePublishedDto publishedDto) //TOWATCH
        {
            if (publishedDto.ImageType != ImageType.PRODUCT_IMAGE)
                await Task.FromException(new Exception("ImageType doesn't match"));

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();          

            var product = new Product
            {
                ProductId = publishedDto.Id,
                ImageURLs = [publishedDto.Url],
                UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(_config)
            };

            var res = await repo.AddImagesToProductAsync(product);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);
            }
        }      

        private async Task DeleteProductsByVendorId(VendorPublishedDto publishedDto) //TOWATCH
        {
            using var scope = _scopeFactory.CreateScope();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepo>();           

            var res = await productRepo.DeleteProductsByOwnerIdAsync(publishedDto.VendorId);

            if (res.success)
            {               
                var productsPub = _mapper.Map<List<ProductPublishedDto>>(res.Value);

                //await _messageBusClient.PublishGenericEvent(productsPub, EventType.ProductsDeletePublished,
                //    [ServicesEnum.SEARCH_SERVICE, ServicesEnum.REVIEW_SERVICE]);

                List<ImagePublishedDto> publishedDtos = [];

                foreach (var product in res.Value)
                {
                    if (res.Value.Any(p => p.ImageURLs.Count > 0))
                    {                      
                        foreach (var url in product.ImageURLs)
                            publishedDtos.Add(new ImagePublishedDto { Id = product.ProductId, ImageType = ImageType.PRODUCT_IMAGE, Url = url });
                    }
                }

               // await _messageBusClient.PublishGenericEvent(publishedDtos, EventType.ImageDeletePublished, [ServicesEnum.IMAGE_SERVICE]);
            }
        }
    }
}
