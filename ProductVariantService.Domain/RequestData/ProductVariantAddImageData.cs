using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.RequestData;

public sealed record ProductVariantAddImageData(
    ImageUrl Url,
    AltText Alt);