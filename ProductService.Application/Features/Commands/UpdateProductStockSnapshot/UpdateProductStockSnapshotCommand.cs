using MediatR;
using migApp.Shared.Results;
using ProductService.Domain.Enums;

namespace ProductService.Application.Features.Commands.UpdateProductStockSnapshot;

public sealed record UpdateProductStockSnapshotCommand(
    Guid ProductId, 
    Guid VendorId,
    StockStatus Status,
    int AvailableQuantity) : IRequest<IResult>;