using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductAttributesReplacedDomainEvent(Guid ProductId) : IDomainEvent;