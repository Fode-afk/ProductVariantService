using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.UpdateProductVariantInfo;

public sealed record UpdateProductVariantInfoCommand(
    Guid VendorId,
    Guid ProductVariantId,

    string SKU,
    string Barcode,

    DimensionsDto Dimensions,
    WeightDto Weight) : IRequest<IResult>;