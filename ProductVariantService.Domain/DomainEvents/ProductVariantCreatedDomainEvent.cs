using migApp.Shared.Domain.Primitives;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantCreatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    Sku Sku,
    Dimensions Dimensions,
    Weight Weight,
    Barcode Barcode,
    bool HasMainImage,
    IReadOnlyList<VariantAttribute> Attributes,
    long Version) : IDomainEvent;