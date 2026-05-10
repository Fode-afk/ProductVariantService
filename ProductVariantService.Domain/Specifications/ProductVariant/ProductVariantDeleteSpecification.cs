using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantDeleteSpecification
{
    public static readonly ISpecification<ProductVariantDeleteContext> Spec =
        new VendorIsActiveSpec<ProductVariantDeleteContext>()
            .And(new ProductCanBeModifiedSpec<ProductVariantDeleteContext>());
}
