namespace ProductService.Domain.Models;

public sealed class ProductCardSnapshot
{
    public Guid ProductCardId { get; set; }
    public Guid VendorId { get; set; }
    public ProductCardStatus Status { get; set; }
}

public enum ProductCardStatus
{
    Draft,
    Published,
    Archived
}