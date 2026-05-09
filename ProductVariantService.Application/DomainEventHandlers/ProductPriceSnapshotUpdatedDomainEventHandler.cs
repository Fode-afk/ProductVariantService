using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.DomainEventHandlers;

//public sealed class ProductPriceSnapshotUpdatedDomainEventHandler(IAppDbContext context) : //IPreCommitDomainEventHandler<ProductPriceSnapshotUpdatedDomainEvent>
//{
//    public async Task Handle(ProductPriceSnapshotUpdatedDomainEvent notification, CancellationToken //cancellationToken)
//    {
//        var productReadModel = await context.ProductReadModels
//            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);
//
//        if (productReadModel == null)
//            return;
//
//        productReadModel.PriceAmount = notification.PriceSnapshot.Amount;
//        productReadModel.OldPriceAmount = notification.OldPriceSnapshot?.Amount;
//        productReadModel.PriceUpdatedAt = notification.PriceUpdatedAt;
//        productReadModel.UpdatedAt = notification.UpdatedAt;
//    }
//}
