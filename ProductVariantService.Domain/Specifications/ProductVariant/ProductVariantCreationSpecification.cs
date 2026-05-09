using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantCreationSpecification
{
    public static readonly ISpecification<ProductVariantCreationContext> Spec =
        new VendorIsActiveSpec<ProductVariantCreationContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantCreationContext>())
            .And(new AttributesRequiredSpec<ProductVariantCreationContext>())
            .And(new AttributesLimitSpec<ProductVariantCreationContext>());
}
