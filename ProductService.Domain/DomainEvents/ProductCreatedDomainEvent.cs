using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent;