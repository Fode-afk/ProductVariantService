using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.AddProductVariantImage;

public sealed record AddProductVariantImageCommand(
    Guid ProductVariantId, 
    Guid VendorId,
    string ImageUrl,
    string AltText) : IRequest<IResult>;