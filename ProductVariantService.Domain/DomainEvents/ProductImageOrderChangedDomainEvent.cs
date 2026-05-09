using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductImageOrderChangedDomainEvent(
    Guid ProductId, 
    Guid ProductCardId,
    IReadOnlyList<ProductVariantImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;