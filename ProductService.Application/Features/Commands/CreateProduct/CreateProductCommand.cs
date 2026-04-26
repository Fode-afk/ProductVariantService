using MediatR;
using migApp.Shared.Results;
using ProductService.Application.Dtos;

namespace ProductService.Application.Features.Commands.CreateProduct;

public sealed record CreateProductCommand(
    Guid VendorId,
    Guid ProductCardId, 
    string Name,

    DimensionsDto Dimensions, 
    WeightDto Weight,

    string Barcode,
    string SKU,

    Dictionary<string, string> Attributes,
    List<string> Tags,

    bool IsDefault) : IRequest<IResult>;