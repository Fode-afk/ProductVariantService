using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductMarkAsDefaultDomainEvent(Guid ProductId, Guid ProductCardId) : IDomainEvent;