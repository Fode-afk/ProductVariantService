using ProductVariantService.Domain.Abstractions;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantAddImageContext(
    bool VendorIsActive,
    bool ProductCanEditContent,
    int ImagesCount) : IVendorContext, IProductContext;