using migApp.Shared.Domain.Primitives;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantImageAltUpdatedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyList<ProductVariantImage> Images,
    long Version) : IDomainEvent;