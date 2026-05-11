using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantCreatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    bool HasMainImage,
    long Version,
    IReadOnlyList<VariantAttribute> Attributes) : IDomainEvent;