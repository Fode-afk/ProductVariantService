using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantRemoveImageContext(
    bool VendorIsActive,
    bool ProductCanEditContent) : IVendorContext, IProductContext;