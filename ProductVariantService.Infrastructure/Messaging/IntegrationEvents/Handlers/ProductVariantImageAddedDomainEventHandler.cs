using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantImageAddedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantImageAddedDomainEvent>
{
    public async Task Handle(ProductVariantImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantImageAddedIntegrationEvent(
            notification.ProductVariantId,
            notification.HasMainImage,
            notification.Version), cancellationToken);
}
