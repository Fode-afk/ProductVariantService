using MediatR;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed record AddVendorSnapshotCommand(
    Guid VendorId,
    bool IsActive,
    long Version) : IRequest;