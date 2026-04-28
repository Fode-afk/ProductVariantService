using ProductService.Domain.Models;
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageAddedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    IReadOnlyList<ProductImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;