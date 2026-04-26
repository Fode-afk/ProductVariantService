namespace ProductService.Domain.Models;

public sealed class ProductReadModel
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

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}