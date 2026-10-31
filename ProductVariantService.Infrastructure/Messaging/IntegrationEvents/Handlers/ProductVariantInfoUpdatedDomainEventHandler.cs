using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantInfoUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantInfoUpdatedDomainEvent>
{
    public async Task Handle(ProductVariantInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantInfoUpdatedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId,
            notification.SKU,
            new DimensionsDto(
                notification.Dimensions.Length,
                notification.Dimensions.Width,
                notification.Dimensions.Height,
                notification.Dimensions.Unit),
            new WeightDto(
                notification.Weight.Value,
                notification.Weight.Unit),
            notification.Barcode,
            notification.Version), cancellationToken);
}