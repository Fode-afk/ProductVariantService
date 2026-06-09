using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageRemovedDomainEvent(
    Guid ProductVariantId,
    bool HasMainImage,
    long Version) : IDomainEvent;