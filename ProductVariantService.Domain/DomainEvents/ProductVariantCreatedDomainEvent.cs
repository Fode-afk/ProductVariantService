using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantCreatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    bool HasMainImage,
    IReadOnlyList<VariantAttribute> Attributes) : IDomainEvent;