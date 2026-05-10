using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantRemoveImageSpecification
{
    public static readonly ISpecification<ProductVariantRemoveImageContext> Spec =
        new VendorIsActiveSpec<ProductVariantRemoveImageContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantRemoveImageContext>());
}