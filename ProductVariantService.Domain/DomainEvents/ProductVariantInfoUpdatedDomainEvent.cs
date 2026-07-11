using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantInfoUpdatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    Sku SKU,
    Dimensions Dimensions,
    Weight Weight,
    Barcode Barcode,
    long Version) : IDomainEvent;