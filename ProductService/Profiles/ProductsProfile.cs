using AutoMapper;
using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ProductGrpc, Product>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes.ToList()))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs.ToList()));

            CreateMap<Product, ProductGrpc>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs));

            //CreateMap<ProductAttributeGrpc, ProductAttribute>().ReverseMap();

            CreateMap<UpdateProductRequest, Product>()
               .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
