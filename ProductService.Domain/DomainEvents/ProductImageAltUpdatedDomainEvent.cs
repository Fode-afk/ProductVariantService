using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageAltUpdatedDomainEvent(Guid ProductId) : IDomainEvent;