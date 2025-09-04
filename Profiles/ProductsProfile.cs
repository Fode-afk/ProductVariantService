using AutoMapper;
using migApp.Shared.Dtos.Products;
using migApp.Shared.EventDtos;
using ProductService.Models;
using ProductService.Protos;
using ProductService.Utils;

namespace ProductService.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ProductGrpc, Product>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes.ToList()))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs.ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeUtil.ParseDate(src.CreatedAt)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeUtil.ParseDate(src.UpdatedAt)));

            CreateMap<Product, ProductGrpc>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("o")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("o")));

            CreateMap<ProductAttributeGrpc, ProductAttribute>().ReverseMap();
            CreateMap<ProductAttributeDto, ProductAttribute>().ReverseMap();

            CreateMap<Product, ProductPublishedDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProductRequest, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateParentCardIdRequest, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddAttributesToProductRequest, Product>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<DeleteAttributesFromProductRequest, Product>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src =>
                    src.Key.Select(k => new ProductAttribute { Key = k }).ToList()))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
