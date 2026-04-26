using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.AddProductCardSnapshot;

public sealed record AddProductCardSnapshotCommand(Guid ProductCardId, Guid VendorId) : IRequest<IResult>;