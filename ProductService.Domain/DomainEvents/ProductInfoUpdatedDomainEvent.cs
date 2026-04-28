using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductInfoUpdatedDomainEvent(
    Guid ProductId, 
    Guid ProductCardId,
    Name Name,
    Dimensions Dimensions,
    Weight Weight,
    DateTimeOffset UpdatedAt) : IDomainEvent;