using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

public sealed record RemoveProductVariantImageCommand(
    Guid ProductVariantId,
    Guid VendorId,
    Guid ImageId) : IRequest<IResult>;