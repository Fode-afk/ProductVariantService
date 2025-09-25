using AutoMapper;
using Grpc.Core;
using migApp.Shared.MsgBus;
using migApp.Shared.MsgBus.Dtos.Images;
using migApp.Shared.MsgBus.Dtos.Product;
using migApp.Shared.MsgBus.Enums;
using MongoDB.Bson;
using ProductService.Data;
using ProductService.Models;
using ProductService.Protos;
using ProductService.Utils;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper, IMessageBusClient messageBusClient,
        IConfiguration config, ILogger logger) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;      
        private readonly IMapper _mapper = mapper;
        private readonly IMessageBusClient _messageBusClient = messageBusClient;
        private readonly IConfiguration _config = config;
        private readonly ILogger _logger = logger;      

        public async override Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetProductsByIds\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");
            
            var res = await _productRepo.GetProductsByIdsAsync([.. request.ProductsIds]);          
            
            return new GetProductsByIdsResponse
            { 
                Status = new StatusResponse { Status = res.success, Reason = res.message },
                Products = { _mapper.Map<IEnumerable<ProductGrpc>>(res.Value) }
            };
        }

        public override async Task<GetAllProductsByOwnerIdResponse> GetAllProductsByOwnerId(GetAllProductsByOwnerIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetAllProductsByOwnerId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");
           
            var res = await _productRepo.GetProductsByOwnerIdAsync(request.OwnerId);

            if (!res.success)
            {
                return new GetAllProductsByOwnerIdResponse { Status = new StatusResponse { Status = res.success, Reason = res.message} };
            }

            return new GetAllProductsByOwnerIdResponse
            {
                Status = new StatusResponse { Status = true },
                Products = { _mapper.Map<IEnumerable<ProductGrpc>>(res.Value) }
            };
        }

        public override async Task<CreateProductModelResponse> CreateProductModel(CreateProductModelRequest request, ServerCallContext context)
        {
            _logger.Log($"\"CreateProductModel\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            var res = await _productRepo.CreateProductModelAsync(request.OwnerId, localTime);

            return new CreateProductModelResponse { Status = new StatusResponse { Status = res.success, Reason = res.message }, ProductId = res.Value };
        }

        public override async Task<StatusResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"CreateProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            var product = _mapper.Map<Product>(request);
            product.UpdatedAt = localTime;
        
            var res = await _productRepo.CreateProductAsync(product);

            //CardService call
            //Вообще тут должна быть достаточно крепкая связь между продуктом и карточкой.
            //По сути, карточка становится главней продукта 
            
            if (!res.success)
                return new StatusResponse { Status = false, Reason = res.message };

            await _messageBusClient.PublishEventAsync(_mapper.Map<ProductPublishedDto>(res.Value), ProductEvents.Published,
                [ServicesEnum.SEARCH_SERVICE, ServicesEnum.REVIEW_SERVICE, ServicesEnum.RECOMMENDATION_SERVICE]);     

            return new StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<StatusResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"UpdateProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");           

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.UpdateProductAsync(product);
            
            if (res.success)
            {              
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishEventAsync(productPub, ProductEvents.Updated, 
                    [ServicesEnum.SEARCH_SERVICE, ServicesEnum.RECOMMENDATION_SERVICE]);
            }

            return new StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<StatusResponse> DeleteProductsByIds(DeleteProductsRequest request, ServerCallContext context)
        {    
            _logger.Log($"\"DeleteProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");                     

            var res = await _productRepo.DeleteProductsAsync(request.ProductsIds.ToArray());

            if (res.success)
            {
                foreach (var product in res.Value)
                {
                    var eventDto = _mapper.Map<ProductDeletedDto>(product);


                    await _messageBusClient.PublishEventAsync(eventDto, ProductEvents.Deleted,
                    [.. new object[]
                    {
                        ServicesEnum.SEARCH_SERVICE,
                        ServicesEnum.REVIEW_SERVICE,
                        ServicesEnum.CARD_SERVICE,
                        product.ParentCardId != string.Empty ? ServicesEnum.CART_SERVICE : null, //ХЗ
                        ServicesEnum.RECOMMENDATION_SERVICE
                    }
                    .Where(s => s != null)!
                    .Cast<ServicesEnum>()]);

                    if (product.ImageURLs != null && product.ImageURLs.Count > 0)
                    {
                        foreach (var url in product.ImageURLs)
                        {
                            var imageEvent = new ImageDeletedDto { Id = product.ProductId, Url = url };
                            await _messageBusClient.PublishEventAsync(imageEvent, ImageEvents.Deleted, [ServicesEnum.IMAGE_SERVICE]);
                        }
                    }
                }         
            }

            return new StatusResponse { Status = res.success, Reason = res.message };
        }   

        public override async Task<StatusResponse> DeleteImagesFromProduct(DeleteImagesFromProductRequest request, ServerCallContext context) //TOREFACTOR
        {
            _logger.Log($"\"DeleteImagesFromProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = new Product
            {
                ProductId = request.ProductId,
                ImageURLs = [.. request.ImageURLs],
                UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(_config)
            };

            var res = await _productRepo.DeleteImagesFromProductAsync(product);

            if (res.success)
            {
                if (request.ImageURLs.Count > 0)
                {
                    foreach (var url in product.ImageURLs)
                    {
                        var imageEvent = new ImageDeletedDto { Id = product.ProductId, Url = url };
                        await _messageBusClient.PublishEventAsync(imageEvent, ImageEvents.Deleted, [ServicesEnum.IMAGE_SERVICE]);

                    }
                }
            }

            return new StatusResponse { Status = res.success, Reason = res.message };
        }
    }
}
