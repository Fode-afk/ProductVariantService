using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantUpdateImageAltSpecification
{
    public static readonly ISpecification<ProductVariantUpdateImageAltContext> Spec =
        new VendorIsActiveSpec<ProductVariantUpdateImageAltContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantUpdateImageAltContext>());
}
