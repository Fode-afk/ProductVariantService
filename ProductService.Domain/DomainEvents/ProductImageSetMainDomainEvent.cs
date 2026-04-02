using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageSetMainDomainEvent(Guid ProductId) : IDomainEvent;