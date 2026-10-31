namespace ProductVariantService.Domain.Abstractions;

internal interface IVendorContext
{
    bool VendorIsActive { get; }
}
