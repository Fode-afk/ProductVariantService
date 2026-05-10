using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantAddImageContext(
    bool VendorIsActive,
    bool ProductCanBeModified,
    int ImagesCount) : IVendorContext, IProductContext;