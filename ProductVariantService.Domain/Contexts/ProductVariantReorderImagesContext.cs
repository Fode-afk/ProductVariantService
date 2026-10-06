using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantReorderImagesContext(
    bool VendorIsActive,
    bool ProductCanEditContent) : IVendorContext, IProductContext;