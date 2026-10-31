using migApp.Shared.Domain.Primitives;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantImagesReorderedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyList<ProductVariantImage> Images,
    long Version) : IDomainEvent;