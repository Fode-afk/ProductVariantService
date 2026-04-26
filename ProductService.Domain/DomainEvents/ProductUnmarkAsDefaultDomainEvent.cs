using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductUnmarkAsDefaultDomainEvent(Guid ProductId, Guid ProductCardId) : IDomainEvent; 