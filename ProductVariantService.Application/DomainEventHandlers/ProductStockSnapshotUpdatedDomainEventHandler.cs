using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.DomainEventHandlers;

//public sealed class ProductStockSnapshotUpdatedDomainEventHandler(IAppDbContext context) : //IPreCommitDomainEventHandler<ProductStockSnapshotUpdatedDomainEvent>
//{
//    public async Task Handle(ProductStockSnapshotUpdatedDomainEvent notification, CancellationToken //cancellationToken)
//    {
//        var productReadModel = await context.ProductReadModels
//            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);
//
//        if (productReadModel == null)
//            return;
//
//        productReadModel.Status = notification.Status;
//        productReadModel.AvailableQuantity = notification.AvailableQuantitySnapshot;
//        productReadModel.StockUpdatedAt = notification.StockUpdatedAt;
//        productReadModel.UpdatedAt = notification.UpdatedAt;
//    }
//}
