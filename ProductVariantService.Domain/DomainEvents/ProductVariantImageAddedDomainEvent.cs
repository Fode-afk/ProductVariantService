using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAddedDomainEvent(
    Guid ProductVariantId,
    bool HasMainImage,
    long Version) : IDomainEvent;