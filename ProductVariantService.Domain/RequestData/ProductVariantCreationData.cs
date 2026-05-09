using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.RequestData;

public sealed record ProductVariantCreationData(
    Sku Sku,
    Dimensions Dimensions,
    Weight Weight,
    Barcode Barcode,
    List<VariantAttribute> Attributes);