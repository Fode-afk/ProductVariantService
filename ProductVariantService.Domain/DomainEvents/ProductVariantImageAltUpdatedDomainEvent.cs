using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAltUpdatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyList<ProductVariantImage> Images,
    long Version) : IDomainEvent;