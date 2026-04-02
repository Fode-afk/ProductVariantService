using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductInfoUpdatedDomainEvent(Guid ProductId) : IDomainEvent;