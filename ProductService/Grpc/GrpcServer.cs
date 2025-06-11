using AutoMapper;
using Grpc.Core;
using ProductService.Data;
using ProductService.Grpc.Validators;
using ProductService.Models;
using ProductService.Protos;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;

        private readonly IValidator<CreateProductRequest> _createProductValidator = new CreateProductValidator();
        private readonly IValidator<GetProductRequest> _getProductValidator = new GetProductValidator();
        private readonly IValidator<UpdateProductRequest> _updateProductValidator = new UpdateProductValidator();
        private readonly IValidator<DeleteProductRequest> _deleteProductValidator = new DeleteProductValidator();

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

            var product = _mapper.Map<Product>(request.Product);

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

            var existingProduct = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (existingProduct == null)
            {
                return new UpdateProductResponse { Status = false };
            }

            _mapper.Map(request, existingProduct);

            await _productRepo.UpdateProductAsync(existingProduct);

            return new UpdateProductResponse { Status = true };
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
    }
}
