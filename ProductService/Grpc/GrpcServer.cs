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

            //var prodAttributesGrpc = _mapper.Map<IEnumerable<ProductAttributeGrpc>>(product.Attributes);

            return new GetProductResponse() { Status = true, Product = _mapper.Map<ProductGrpc>(product) };
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            //var prodAttributes = _mapper.Map<IEnumerable<ProductAttribute>>(request.Product.Attributes);

            await _productRepo.CreateProductAsync(_mapper.Map<Product>(request.Product));

            return ;
        }

        public override async Task<UpdateProductResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
            await _productRepo.UpdateProductAsync(_mapper.Map<Product>(request));
            return;
        }

        public override async Task<DeleteProductResponse> DeleteProductById(DeleteProductRequest request, ServerCallContext context)
        {
            await _productRepo.DeleteProductAsync(request.ProductId);
            return;
        }
    }
}
