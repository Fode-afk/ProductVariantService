using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductImageAddedDomainEvent(
    Guid ProductVariantId,
    bool HasMainImage,
    long Version) : IDomainEvent;