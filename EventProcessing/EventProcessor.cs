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
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task ProcessEventAsync(string message)
        {
            BaseEventDto<object> baseEvent;
            try
            {
                baseEvent = JsonSerializer.Deserialize<BaseEventDto<object>>(message, _jsonSerializerOptions);
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
                    await HandleImageEvent(message, Enum.Parse<ImageEvents>(baseEvent.EventType));
                    break;
                default:
                    Console.WriteLine($"--> Unknown event category: {baseEvent.Category}");
                    break;
            }
        }

        private async Task HandleImageEvent(string message, ImageEvents eventType)
        {
            object signature;

            try
            {
                signature = eventType switch
                {
                    ImageEvents.Published => JsonSerializer.Deserialize<BaseEventDto<ImagePublishedDto>>(message, _jsonSerializerOptions)!,
                    _ => throw new NotImplementedException()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Product event deserialization failed: {ex.Message}");
                return;
            }

            if (signature == null)
                return;

            switch (eventType)
            {
                case ImageEvents.Published:
                    var publishedDto = (BaseEventDto<ImagePublishedDto>)signature;
                    await AddProductImage(publishedDto.Event);
                    break;              
                default:
                    Console.WriteLine($"--> Unhandled product event type: {signature}");
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

            await repo.AddImagesToProductAsync(product);
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
