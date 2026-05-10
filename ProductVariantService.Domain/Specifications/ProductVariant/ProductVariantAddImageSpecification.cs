using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantAddImageSpecification
{
    public static readonly ISpecification<ProductVariantAddImageContext> Spec =
        new VendorIsActiveSpec<ProductVariantAddImageContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantAddImageContext>())
            .And(Specification<ProductVariantAddImageContext>.Create(
                ctx => ctx.ImagesCount < Models.ProductVariant.MaxImages,
                ProductVariantImageErrors.MaxImagesReached()));
}
