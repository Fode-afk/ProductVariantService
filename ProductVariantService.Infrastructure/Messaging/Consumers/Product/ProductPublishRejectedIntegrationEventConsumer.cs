using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductPublishRejectedIntegrationEventConsumer(ISender sender) : IConsumer<ProductPublishRejectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPublishRejectedIntegrationEvent> context) =>
        await sender.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CategoryId,
            context.Message.CanEditContent,
            context.Message.Version), context.CancellationToken);
}