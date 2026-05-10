using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantDeletedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantDeletedDomainEvent>
{
    public async Task Handle(ProductVariantDeletedDomainEvent notification, CancellationToken cancellationToken) => 
        await publish.Publish(new ProductVariantDeletedIntegrationEvent(notification.ProductVariantId), cancellationToken);
}
