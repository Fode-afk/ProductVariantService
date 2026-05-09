namespace ProductVariantService.Domain.Abstractions;

public interface IVendorContext
{
    bool VendorIsActive { get; }
}
