using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Primitives;

namespace ProductService.Application.DomainEventHandlers;

public sealed class ProductUnmarkAsDefaultDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductUnmarkAsDefaultDomainEvent>
{
    public async Task Handle(ProductUnmarkAsDefaultDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productReadModel == null)
            return;

        productReadModel.IsDefault = notification.IsDefault;
        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
