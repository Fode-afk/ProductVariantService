using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantImagesReorderedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyList<ProductVariantImage> Images,
    long Version) : IDomainEvent;