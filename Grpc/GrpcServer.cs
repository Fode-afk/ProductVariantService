using AutoMapper;
using Grpc.Core;
using MongoDB.Bson;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.Dtos;
using ProductService.EventProcessing;
using ProductService.Grpc.Validators;
using ProductService.Models;
using ProductService.Protos;
using ProductService.Utils;
using ReviewService.Protos;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper, IMessageBusClient messageBusClient,
        IConfiguration config, ILogger logger, GrpcReviews.GrpcReviewsClient reviewsClient) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IMessageBusClient _messageBusClient = messageBusClient;
        private readonly IConfiguration _config = config;
        private readonly ILogger _logger = logger;
        private readonly GrpcReviews.GrpcReviewsClient _reviewsClient = reviewsClient;

        private readonly IValidator<CreateProductRequest> _createProductValidator = new CreateProductValidator();
        private readonly IValidator<GetProductRequest> _getProductValidator = new GetProductValidator();
        private readonly IValidator<GetAllProductsByOwnerIdRequest> _getAllProductsValidator = new GetAllProductsValidator();
        private readonly IValidator<UpdateProductRequest> _updateProductValidator = new UpdateProductValidator();
        private readonly IValidator<DeleteProductRequest> _deleteProductValidator = new DeleteProductValidator();

        public async override Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetProductsByIds\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.GetProductsByIdsAsync([.. request.ProductIds]);

            if (!res.success)
            {
                return new GetProductsByIdsResponse { Status = new Protos.StatusResponse { Status = res.success, Reason = res.message } };
            }
            return new GetProductsByIdsResponse { Status = new Protos.StatusResponse { Status = res.success }, Products = { _mapper.Map<IEnumerable<ProductGrpc>>(res.Value) } };
        }

        public override async Task<GetAllProductsByOwnerIdResponse> GetAllProductsByOwnerId(GetAllProductsByOwnerIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetAllProductsByOwnerId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _getAllProductsValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var res = await _productRepo.GetAllProductsByOwnerIdAsync(request.OwnerId);

            if (!res.success)
            {
                return new GetAllProductsByOwnerIdResponse { Status = new Protos.StatusResponse { Status = res.success, Reason = res.message} };
            }

            return new GetAllProductsByOwnerIdResponse
            {
                Status = new Protos.StatusResponse { Status = true },
                Products = { _mapper.Map<IEnumerable<ProductGrpc>>(res.Value) }
            };
        }

        public override async Task<GetProductResponse> GetProductById(GetProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _getProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var res = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (!res.success)
            {
                return new GetProductResponse { Status = new Protos.StatusResponse { Status = res.success, Reason = res.message } };
            }

            return new GetProductResponse
            {
                Status = new Protos.StatusResponse { Status = res.success },
                Product = _mapper.Map<ProductGrpc>(res.Value)
            };
        }

        public override async Task<Protos.StatusResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"CreateProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _createProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = _mapper.Map<Product>(new ProductGrpc() { Name = request.Name, OwnerId = request.OwnerId });

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.CreatedAt = localTime;
            product.UpdatedAt = localTime;

            var res = await _productRepo.CreateProductAsync(product);
            
            if (!res.success)
                return new Protos.StatusResponse { Status = false, Reason = res.message };

            try
            {
                var ratingRes = await _reviewsClient.CreateRatingAsync(new CreateRatingRequest { Id = product.ProductId });

                if (!ratingRes.Status)
                {
                    await _productRepo.DeleteProductAsync(product.ProductId);
                    return new Protos.StatusResponse { Status = false, Reason = res.message };                 
                }
            }
            catch (Exception ex)
            {
                await _productRepo.DeleteProductAsync(product.ProductId);
                return new Protos.StatusResponse { Status = false, Reason = ex.Message };
            }

            var productPub = _mapper.Map<ProductPublishedDto>(product);

            await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductPublished, [ServicesEnum.SEARCH_SERVICE]);           

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<Protos.StatusResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"UpdateProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _updateProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.UpdateProductAsync(product);
            
            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<Protos.StatusResponse> SetCanBeOrdered(SetCanBeOrderedRequest request, ServerCallContext context)
        {
            _logger.Log($"\"SetCanBeOrdered\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");          
            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            var res = await _productRepo.SetCanBeOrderedAsync(request.ProductId, request.CanBeOrdered, localTime);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<Protos.StatusResponse> DeleteProductById(DeleteProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"DeleteProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _deleteProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var res = await _productRepo.DeleteProductAsync(request.ProductId);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(new Product
                {
                    ProductId = request.ProductId,
                    ParentCardId = res.Value.ParentCardId
                });

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductDeletePublished,
                    [.. new object[]
                    {
                        ServicesEnum.SEARCH_SERVICE,
                        ServicesEnum.REVIEW_SERVICE,
                        res.Value.ParentCardId != string.Empty ? ServicesEnum.CARD_SERVICE : null,
                        res.Value.ParentCardId != string.Empty ? ServicesEnum.CART_SERVICE : null
                    }
                    .Where(s => s != null)!
                    .Cast<ServicesEnum>()]);

                if (res.Value.ImageURLs.Count > 0)
                {
                    List<ImagePublishedDto> publishedDtos = [];

                    foreach (var url in res.Value.ImageURLs)
                        publishedDtos.Add(new ImagePublishedDto { Id = request.ProductId, ContentType = ContentType.PRODUCT_IMAGE, Url = url });

                    await _messageBusClient.PublishGenericEvent(publishedDtos, EventType.ImageDeletePublished, [ServicesEnum.IMAGE_SERVICE]);
                }
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<Protos.StatusResponse> ProductExists(ProductExistsRequest request, ServerCallContext context)
        {
            _logger.Log($"\"ProductExists\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.ProductExistsAsync(request.ProductId);
            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<GetParentCardIdResponse> GetParentCardId(GetParentCardIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetParentCardId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.GetParentCardIdAsync(request.ProductId);

            if (res.Value == "")
            { 
                return new GetParentCardIdResponse {Status = new Protos.StatusResponse { Status = res.success, Reason = res.message } };
            }

            return new GetParentCardIdResponse {Status = new Protos.StatusResponse { Status = res.success, Reason = res.message }, ParentCardId = res.Value };
        }

        public override async Task<Protos.StatusResponse> UpdateParentCardId(UpdateParentCardIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"UpdateParentCardId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.UpdateParentCardIdAsync(product);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdateParentCardIdPublished, [ServicesEnum.SEARCH_SERVICE]);
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<GetOwnerIdResponse> GetOwnerId(GetOwnerIdRequest request, ServerCallContext context)
        {
            var res = await _productRepo.GetOwnerIdAsync(request.ProductId);

            return new GetOwnerIdResponse {
                Status = new Protos.StatusResponse
                { 
                    Status = res.success,
                    Reason = res.message
                },
                OwnerId = res.Value
            };
        }
        public override async Task<Protos.StatusResponse> AddAttributesToProduct(AddAttributesToProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"AddAttributesToProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.AddAttributesToProductAsync(product);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(res.Value);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<Protos.StatusResponse> DeleteAttributesFromProduct(DeleteAttributesFromProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"DeleteAttributesFromProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.DeleteAttributesFromProductAsync(product);

            if (res.success)
            {
                var productPub = _mapper.Map<ProductPublishedDto>(product);

                await _messageBusClient.PublishGenericEvent(productPub, EventType.ProductUpdatePublished, [ServicesEnum.SEARCH_SERVICE]);            
            }

            return new Protos.StatusResponse { Status = res.success, Reason = res.message };
        }
    }
}
