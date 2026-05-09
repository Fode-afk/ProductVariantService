using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.RemoveProductImage;

public sealed record RemoveProductImageCommand(
    Guid ProductId,
    Guid VendorId,
    string Url) : IRequest<IResult>;