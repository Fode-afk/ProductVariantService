using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Specifications.Base;
using ProductVariantService.Domain.Specifications.Common;

namespace ProductVariantService.Domain.Specifications.ProductVariant;

public static class ProductVariantUpdateInfoSpecification
{
    public static readonly ISpecification<ProductVariantUpdateInfoContext> Spec =
        new VendorIsActiveSpec<ProductVariantUpdateInfoContext>();
}