using MediatR;
using migApp.Shared.Enums.Inventory;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.UpdateProductStockSnapshot;

public sealed record UpdateProductStockSnapshotCommand(
    Guid ProductId, 
    Guid VendorId,
    StockStatus Status,
    int AvailableQuantity) : IRequest<IResult>;