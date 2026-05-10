using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed record DeleteVendorSnapshotCommand(Guid VendorId) : IRequest<IResult>;