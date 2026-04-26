using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageRemovedDomainEvent(Guid ProductId, Guid ProductCardId) : IDomainEvent;