using AutoMapper;
using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Profiles
{
    /// <summary>
    /// Defines AutoMapper configuration for mapping between Product-related gRPC models and domain models.
    /// </summary>
    public class ProductsProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsProfile"/> class and sets up mapping rules.
        /// </summary>
        public ProductsProfile()
        {
            /// <summary>
            /// Maps from gRPC product model to domain model, ensuring list properties are correctly converted.
            /// </summary>
            CreateMap<ProductGrpc, Product>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes.ToList()))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs.ToList()));

            /// <summary>
            /// Maps from domain product model to gRPC model.
            /// </summary>
            CreateMap<Product, ProductGrpc>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.ImageURLs, opt => opt.MapFrom(src => src.ImageURLs));

            /// <summary>
            /// Enables two-way mapping between product attribute domain and gRPC models.
            /// </summary>
            CreateMap<ProductAttributeGrpc, ProductAttribute>().ReverseMap();

            /// <summary>
            /// Maps update requests to product model, ignoring null values to allow partial updates.
            /// </summary>
            CreateMap<ReplaceProductRequest, Product>()
               .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<UpdateProductRequest, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
