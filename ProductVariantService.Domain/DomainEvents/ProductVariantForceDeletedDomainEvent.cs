using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantForceDeletedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId) : IDomainEvent;
