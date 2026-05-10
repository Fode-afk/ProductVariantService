using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

public sealed record UpdateProductSnapshotCommand(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    long Version) : IRequest<IResult>;