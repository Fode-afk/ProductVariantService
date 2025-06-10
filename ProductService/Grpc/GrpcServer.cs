using AutoMapper;
using Grpc.Core;
using ProductService.Data;
using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Grpc
{
    public class GrpcServer(IProductRepo productRepo, IMapper mapper) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;

        public override async Task<GetProductResponse> GetProductById(GetProductRequest request, ServerCallContext context)
        {
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
            if (request.Product == null)
            {
                return new CreateProductResponse { Status = false };
            }

            var product = _mapper.Map<Product>(request.Product);

            await _productRepo.CreateProductAsync(product);

            return new CreateProductResponse { Status = true };
        }

        public override async Task<UpdateProductResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
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
            await _productRepo.DeleteProductAsync(request.ProductId);

            return new DeleteProductResponse { Status = true };
        }
    }
}
