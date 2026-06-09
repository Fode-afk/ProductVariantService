using MediatR;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed record DeleteVendorSnapshotCommand(Guid VendorId) : IRequest;