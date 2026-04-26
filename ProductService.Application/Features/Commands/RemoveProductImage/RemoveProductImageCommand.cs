using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.RemoveProductImage;

public sealed record RemoveProductImageCommand(
    Guid ProductId,
    Guid VendorId,
    string Url) : IRequest<IResult>;