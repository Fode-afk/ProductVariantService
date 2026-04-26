using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.SetMainProductImage;

public sealed record SetMainProductImageCommand(
    Guid ProductId,
    Guid VendorId,
    string Url) : IRequest<IResult>;