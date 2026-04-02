using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductSetDefaultDomainEvent(Guid ProductId) : IDomainEvent;