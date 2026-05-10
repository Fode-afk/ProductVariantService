namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVendorOwnershipContext(
    Guid RequestVendorId,
    Guid ProductVendorId);
