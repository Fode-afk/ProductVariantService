using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.CreateProductVariant;

public sealed record CreateProductVariantCommand(
    Guid VendorId,
    Guid ProductId, 

    DimensionsDto Dimensions, 
    WeightDto Weight,

    string Barcode,
    string SKU,

    Dictionary<Guid, string> Attributes) : IRequest<IResult>;