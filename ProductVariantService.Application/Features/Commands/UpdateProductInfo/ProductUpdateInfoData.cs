using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Application.Features.Commands.UpdateProductInfo;

internal sealed record ProductUpdateInfoData(
    Dimensions Dimensions,
    Weight Weight);