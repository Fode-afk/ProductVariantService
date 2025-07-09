using AutoMapper;
using Grpc.Core;
using MongoDB.Bson;
using ProductService.Data;
using ProductService.Grpc.Validators;
using ProductService.Models;
using ProductService.Protos;
using ProductService.Utils;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper, IConfiguration config, ILogger logger) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;
        private readonly ILogger _logger = logger;

        private readonly IValidator<CreateProductRequest> _createProductValidator = new CreateProductValidator();
        private readonly IValidator<GetProductRequest> _getProductValidator = new GetProductValidator();
        private readonly IValidator<GetAllProductsByOwnerIdRequest> _getAllProductsValidator = new GetAllProductsValidator();
        private readonly IValidator<UpdateProductRequest> _updateProductValidator = new UpdateProductValidator();
        private readonly IValidator<DeleteProductRequest> _deleteProductValidator = new DeleteProductValidator();

        public async override Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetProductsByIds\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.GetProductsByIdsAsync([.. request.ProductIds]);

            if (res == null)
            {
                return new GetProductsByIdsResponse { Status = res.success, Reason = res.message };
            }
            return new GetProductsByIdsResponse { Status = res.success, Products = { _mapper.Map<IEnumerable<ProductGrpc>>(res.Value) } };
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

            if (!res.Value.Any())
            {
                return new GetAllProductsByOwnerIdResponse { Status = res.success, Reason = res.message };
            }

            return new GetAllProductsByOwnerIdResponse
            {
                Status = res.success,
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
                return new GetProductResponse { Status = res.success, Reason = res.message };
            }

            return new GetProductResponse
            {
                Status = res.success,
                Product = _mapper.Map<ProductGrpc>(res.Value)
            };
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
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

            return new CreateProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<UpdateProductResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
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

            return new UpdateProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<DeleteProductResponse> DeleteProductById(DeleteProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"DeleteProductById\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var valres = _deleteProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var res = await _productRepo.DeleteProductAsync(request.ProductId);

            return new DeleteProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<ProductExistsResponse> ProductExists(ProductExistsRequest request, ServerCallContext context)
        {
            _logger.Log($"\"ProductExists\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.ProductExistsAsync(request.ProductId);
            return new ProductExistsResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<GetParentCardIdResponse> GetParentCardId(GetParentCardIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"GetParentCardId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var res = await _productRepo.GetParentCardIdAsync(request.ProductId);

            if (res.Value == "")
            { 
                return new GetParentCardIdResponse { Status = res.success, Reason = res.message };
            }

            return new GetParentCardIdResponse { Status = res.success, Reason = res.message, ParentCardId = res.Value };
        }

        public override async Task<UpdateParentCardIdResponse> UpdateParentCardId(UpdateParentCardIdRequest request, ServerCallContext context)
        {
            _logger.Log($"\"UpdateParentCardId\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.UpdateParentCardId(product);

            return new UpdateParentCardIdResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<AddImagesToProductResponse> AddImagesToProduct(AddImagesToProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"AddImagesToProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.AddImagesToProductAsync(product);

            return new AddImagesToProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<DeleteImagesFromProductResponse> DeleteImagesFromProduct(DeleteImagesFromProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"DeleteImagesFromProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.DeleteImagesFromProductAsync(product);

            return new DeleteImagesFromProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<AddAttributesToProductResponse> AddAttributesToProduct(AddAttributesToProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"AddAttributesToProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.AddAttributesToProductAsync(product);

            return new AddAttributesToProductResponse { Status = res.success, Reason = res.message };
        }

        public override async Task<DeleteAttributesFromProductResponse> DeleteAttributesFromProduct(DeleteAttributesFromProductRequest request, ServerCallContext context)
        {
            _logger.Log($"\"DeleteAttributesFromProduct\" with params {request.ToJson()} has noticed. Caller: {context.Peer}");

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var res = await _productRepo.DeleteAttributesFromProductAsync(product);

            return new DeleteAttributesFromProductResponse { Status = res.success, Reason = res.message };
        }
    }
}
