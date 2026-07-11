using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantDeletedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId) : IDomainEvent;