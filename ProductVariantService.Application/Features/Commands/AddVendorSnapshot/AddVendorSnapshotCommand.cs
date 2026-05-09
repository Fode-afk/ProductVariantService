using MediatR;
using migApp.Shared.Enums.Vendors;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.AddVendorSnapshot;

public sealed record AddVendorSnapshotCommand(
    Guid VendorId,
    VendorStatus Status,
    bool IsVerified) : IRequest<IResult>;