using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantReorderImagesSpecification
{
    public static readonly ISpecification<ProductVariantReorderImagesContext> Spec =
        new VendorIsActiveSpec<ProductVariantReorderImagesContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantReorderImagesContext>());
}
