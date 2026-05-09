namespace ProductVariantService.Application.Dtos;

public sealed record DimensionsDto(
    double Length,
    double Width,
    double Height,
    string Unit);