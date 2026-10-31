using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantCreatedDomainEvent>
{
    public async Task Handle(ProductVariantCreatedDomainEvent notification, CancellationToken cancellationToken)
    { 
        await publish.Publish(new ProductVariantCreatedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId,
            notification.Sku,
            new DimensionsDto(
                notification.Dimensions.Length,
                notification.Dimensions.Width,
                notification.Dimensions.Height,
                notification.Dimensions.Unit),
            new WeightDto(
                notification.Weight.Value,
                notification.Weight.Unit),
            notification.Barcode,
            notification.HasMainImage,
            [.. notification.Attributes.Select(a =>
                new VariantAttributeDto(
                    a.CharacteristicId,
                    a.Name,
                    a.Value,
                    a.CharType,
                    a.GroupName?.Value))],
            notification.Version), cancellationToken);
    }
}