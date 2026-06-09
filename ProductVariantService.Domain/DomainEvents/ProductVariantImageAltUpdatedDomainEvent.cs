using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAltUpdatedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    IReadOnlyList<ProductVariantImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;