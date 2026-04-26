using MediatR;
using migApp.Shared.Results;
using ProductService.Application.Dtos;

namespace ProductService.Application.Features.Commands.UpdateProductInfo;

public sealed record UpdateProductInfoCommand(
    Guid VendorId,
    Guid ProductId,
    string Name,
    DimensionsDto Dimensions,
    WeightDto Weight) : IRequest<IResult>;