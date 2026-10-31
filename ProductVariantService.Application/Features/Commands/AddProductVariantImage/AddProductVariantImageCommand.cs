using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.AddProductVariantImage;

public sealed record AddProductVariantImageCommand(
    Guid ProductVariantId, 
    Guid VendorId,
    string ImageUrl,
    string AltText) : IRequest<IResult<ProductVariantWriteAck>>;