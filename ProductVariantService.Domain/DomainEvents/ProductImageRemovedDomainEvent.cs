using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductImageRemovedDomainEvent(
    Guid ProductVariantId,
    bool HasMainImage,
    long Version) : IDomainEvent;