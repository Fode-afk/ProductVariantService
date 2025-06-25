using AutoMapper;
using Grpc.Core;
using ProductService.Data;
using ProductService.Grpc.Validators;
using ProductService.Models;
using ProductService.Protos;
using ProductService.Utils;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper, IConfiguration config) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;

        private readonly IValidator<CreateProductRequest> _createProductValidator = new CreateProductValidator();
        private readonly IValidator<GetProductRequest> _getProductValidator = new GetProductValidator();
        private readonly IValidator<GetAllProductsByOwnerIdRequest> _getAllProductsValidator = new GetAllProductsValidator();
        private readonly IValidator<UpdateProductRequest> _updateProductValidator = new UpdateProductValidator();
        private readonly IValidator<DeleteProductRequest> _deleteProductValidator = new DeleteProductValidator();

        public async override Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request, ServerCallContext context)
        {
            var products = await _productRepo.GetProductsByIdsAsync([.. request.ProductIds]);

            if (products == null)
            {
                return new GetProductsByIdsResponse { Status = false };
            }
            return new GetProductsByIdsResponse { Status = true, Products = { _mapper.Map<IEnumerable<ProductGrpc>>(products) } };
        }

        public override async Task<GetAllProductsByOwnerIdResponse> GetAllProductsByOwnerId(GetAllProductsByOwnerIdRequest request, ServerCallContext context)
        {
            var valres = _getAllProductsValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var products = await _productRepo.GetAllProductsByOwnerIdAsync(request.OwnerId);

            if (!products.Any())
            {
                return new GetAllProductsByOwnerIdResponse { Status = false };
            }

            return new GetAllProductsByOwnerIdResponse
            {
                Status = true,
                Products = { _mapper.Map<IEnumerable<ProductGrpc>>(products) }
            };
        }

        public override async Task<GetProductResponse> GetProductById(GetProductRequest request, ServerCallContext context)
        {
            var valres = _getProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (product == null)
            {
                return new GetProductResponse { Status = false };
            }

            return new GetProductResponse
            {
                Status = true,
                Product = _mapper.Map<ProductGrpc>(product)
            };
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var valres = _createProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = _mapper.Map<Product>(new ProductGrpc() { Name = request.Name, OwnerId = request.OwnerId });

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.CreatedAt = localTime;
            product.UpdatedAt = localTime;

            await _productRepo.CreateProductAsync(product);

            return new CreateProductResponse { Status = true };
        }

        public override async Task<UpdateProductResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
            var valres = _updateProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var result = await _productRepo.UpdateProductAsync(product);

            return new UpdateProductResponse { Status = result };
        }

        public override async Task<DeleteProductResponse> DeleteProductById(DeleteProductRequest request, ServerCallContext context)
        {
            var valres = _deleteProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            await _productRepo.DeleteProductAsync(request.ProductId);

            return new DeleteProductResponse { Status = true };
        }

        public override async Task<ProductExistsResponse> ProductExists(ProductExistsRequest request, ServerCallContext context)
        {
            return new ProductExistsResponse
            {
                Status = await _productRepo.ProductExistsAsync(request.ProductId)
            };
        }

        public override async Task<GetParentCardIdResponse> GetParentCardId(GetParentCardIdRequest request, ServerCallContext context)
        {
            var cardId = await _productRepo.GetParentCardIdAsync(request.ProductId);

            if (cardId == "")
            { 
                return new GetParentCardIdResponse { Status = false };
            }

            return new GetParentCardIdResponse { Status = true, ParentCardId = cardId };
        }

        public override async Task<UpdateParentCardIdResponse> UpdateParentCardId(UpdateParentCardIdRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request);

            var localTime = DateTimeUtil.GetCurrentTimeFormatted(_config);

            product.UpdatedAt = localTime;

            var result = await _productRepo.UpdateParentCardId(product);

            return new UpdateParentCardIdResponse { Status = result };
        }
    }
}
