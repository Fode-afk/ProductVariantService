using MassTransit;
using MassTransit.Mediator;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

public sealed class ProductPublishedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPublishedIntegrationEvent> context) => 
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CategoryId,
            context.Message.CanBeModified,
            context.Message.Version), context.CancellationToken);
}
