using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantDeleteContext(
    bool VendorIsActive,
    bool ProductCanEditContent) : IVendorContext, IProductContext;