namespace ProductVariantService.Application.Dtos;

public sealed record ProductImageDto(
    string Url,
    string Alt,
    bool IsMain,
    int SortOrder);