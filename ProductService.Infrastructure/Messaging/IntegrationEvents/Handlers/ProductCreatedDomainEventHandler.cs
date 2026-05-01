using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Primitives;

namespace ProductService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductCreatedDomainEvent>
{
    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    { 
        await publish.Publish(new ProductCreatedIntegrationEvent(
            notification.Product.Id,
            notification.VendorId,
            notification.Product.ProductCardId), 
            cancellationToken);
    }
}