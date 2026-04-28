using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductAttributesReplacedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    IReadOnlyList<ProductAttribute> Attributes,
    DateTimeOffset UpdatedAt) : IDomainEvent;