using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.UpdateProductImageAlt;

public sealed record UpdateProductImageAltCommand(
    Guid ProductId,
    Guid VendorId,
    string Url,
    string AltText) : IRequest<IResult>;