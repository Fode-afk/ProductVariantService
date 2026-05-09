using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.AddProductImage;

public sealed record AddProductImageCommand(
    Guid ProductId, 
    Guid VendorId,
    string ImageUrl,
    string AltText,
    bool IsMain) : IRequest<IResult>;