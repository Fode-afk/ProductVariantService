using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantUpdateInfoContext(
    bool VendorIsActive,
    bool ProductCanBeModified) : IVendorContext, IProductContext;