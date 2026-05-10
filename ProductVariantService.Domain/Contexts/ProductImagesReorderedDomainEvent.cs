using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductImagesReorderedDomainEvent(
    Guid ProductVariantId,
    Guid ProductId,
    IReadOnlyList<ProductVariantImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;