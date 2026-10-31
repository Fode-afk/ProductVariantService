using migApp.Shared.Domain.Primitives;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAddedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyCollection<ProductVariantImage> Images,
    bool HasMainImage,
    long Version) : IDomainEvent;