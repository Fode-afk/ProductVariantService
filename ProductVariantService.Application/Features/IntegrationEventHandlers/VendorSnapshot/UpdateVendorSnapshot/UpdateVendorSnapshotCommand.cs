using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed record UpdateVendorSnapshotCommand(
    Guid VendorId,
    bool IsActive,
    long Version) : IRequest<IResult>;