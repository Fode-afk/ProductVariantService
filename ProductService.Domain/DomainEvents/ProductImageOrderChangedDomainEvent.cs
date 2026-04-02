using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageOrderChangedDomainEvent(Guid ProductId) : IDomainEvent;