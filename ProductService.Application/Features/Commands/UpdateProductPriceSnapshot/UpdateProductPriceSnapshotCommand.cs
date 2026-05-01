using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.UpdateProductPriceSnapshot;

public sealed record UpdateProductPriceSnapshotCommand(
    Guid ProductId,
    Guid VendorId,
    decimal Price,
    decimal? OldPrice) : IRequest<IResult>;