using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.AddProductSnapshot;

public sealed record AddProductSnapshotCommand(
    Guid ProductId,
    Guid VendorId,
    Guid CategoryId,
    bool CanBeModified) : IRequest<IResult>;