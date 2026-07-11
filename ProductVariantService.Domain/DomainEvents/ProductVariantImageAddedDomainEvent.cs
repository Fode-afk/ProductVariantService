using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAddedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyCollection<ProductVariantImage> Images,
    bool HasMainImage,
    long Version) : IDomainEvent;