using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantCreatedDomainEvent>
{
    public async Task Handle(ProductVariantCreatedDomainEvent notification, CancellationToken cancellationToken)
    { 
        await publish.Publish(new ProductVariantCreatedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId,
            notification.HasMainImage,
            [.. notification.Attributes.Select(a =>
                new ProductVariantCharacteristicValue(
                    notification.ProductVariantId,
                    a.CharacteristicId, 
                    a.Value))]), cancellationToken);
    }
}