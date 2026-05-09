using migApp.Shared.Enums.Inventory;

namespace ProductVariantService.Domain.Models;

public sealed class ProductVariantReadModel
{
    public Guid Id { get; set; }
    public Guid ProductCardId { get; set; }

    public required string SKU { get; set; }
    public required string Name { get; set; }
    public required string NameNormalized { get; set; }
    public required string Barcode { get; set; }

    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public required string DimensionUnit { get; set; }

    public double Weight { get; set; }
    public required string WeightUnit { get; set; }

    public required string MainImage { get; set; }

    public required string AttributesJson { get; set; }
    public required string TagsJson { get; set; }
    public required string TagsFlat { get; set; }
    public required string ImagesJson { get; set; }

    public bool IsDefault { get; set; }

    public decimal? PriceAmount { get; set; }
    public decimal? OldPriceAmount { get; set; }
    public DateTimeOffset? PriceUpdatedAt { get; set; }

    public StockStatus Status { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTimeOffset? StockUpdatedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}