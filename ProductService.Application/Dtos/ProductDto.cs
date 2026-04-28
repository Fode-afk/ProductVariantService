using ProductService.Domain.Enums;

namespace ProductService.Application.Dtos;

public sealed record ProductDto(
    string Id,
    string SKU,
    string Name,
    string Barcode,
    DimensionsDto Dimensions,
    WeightDto Weight,
    long PriceMinorAmount,
    long OldPriceMinorAmount,
    StockStatus Status,
    int AvailableQuantity,
    Dictionary<string, string> Attributes,
    IEnumerable<string> Tags,
    IEnumerable<ProductImageDto> Images);