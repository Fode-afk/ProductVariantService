using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantUpdateImageAltContext(
    bool VendorIsActive,
    bool ProductCanBeModified) : IVendorContext, IProductContext;