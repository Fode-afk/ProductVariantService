using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

public sealed record RemoveProductVariantImageCommand(
    Guid ProductVariantId,
    Guid VendorId,
    Guid ImageId) : IRequest<IResult<ProductVariantWriteAck>>;