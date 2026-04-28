using ProductService.Domain.Models;
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductImageOrderChangedDomainEvent(
    Guid ProductId, 
    Guid ProductCardId,
    IReadOnlyList<ProductImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;