using AutoMapper;
using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ProductGrpc, Product>();
            CreateMap<Product, ProductGrpc>();
        }
    }
}
