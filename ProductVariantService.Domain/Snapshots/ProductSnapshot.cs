namespace ProductVariantService.Domain.Snapshots;

public sealed class ProductSnapshot
{
    public Guid ProductId { get; set; }
    public Guid VendorId { get; set; }
    public Guid CategoryId { get; set; }
    public bool CanBeModified { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}