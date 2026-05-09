using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.UpdateProductInfo;

public sealed record UpdateProductInfoCommand(
    Guid VendorId,
    Guid ProductId,
    string Name,
    DimensionsDto Dimensions,
    WeightDto Weight) : IRequest<IResult>;