using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductService.Models;
using ProductService.Protos;
using System.Globalization;

namespace ProductService.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ProductGrpc, Product>()
             .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes.ToList()))
             .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs.ToList()))
             .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => ParseDate(src.CreatedAt)))
             .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => ParseDate(src.UpdatedAt)));

            CreateMap<Product, ProductGrpc>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("o")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("o")));

            CreateMap<ProductAttributeGrpc, ProductAttribute>().ReverseMap();

            CreateMap<ReplaceProductRequest, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProductRequest, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }

        private DateTimeOffset? ParseDate(string date)
        {
            DateTimeOffset res;
            if (DateTimeOffset.TryParseExact(date, "yyyy-MM-ddTHH:mm:ss.fffK",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
            {
                return res;
            }
            return null;
        }
    }
}
