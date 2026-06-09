using MediatR;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.AddProductSnapshot;

public sealed record AddProductSnapshotCommand(
    Guid ProductId,
    Guid VendorId,
    Guid CategoryId,
    bool CanBeModified,
    long Version) : IRequest;