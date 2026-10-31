using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.UpdateProductVariantImageAlt;

public sealed record UpdateProductVariantImageAltCommand(
    Guid ProductVariantId,
    Guid VendorId,
    Guid ImageId,
    string AltText) : IRequest<IResult<ProductVariantWriteAck>>;