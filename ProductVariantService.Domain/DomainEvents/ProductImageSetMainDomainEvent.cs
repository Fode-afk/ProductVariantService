using migApp.Shared.Domain.Primitives;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductImageSetMainDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    IReadOnlyList<ProductVariantImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;