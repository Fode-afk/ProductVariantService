using MediatR;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

public sealed record UpdateProductSnapshotCommand(
    Guid ProductId,
    Guid CategoryId,
    bool CanEditContent,
    long Version) : IRequest;