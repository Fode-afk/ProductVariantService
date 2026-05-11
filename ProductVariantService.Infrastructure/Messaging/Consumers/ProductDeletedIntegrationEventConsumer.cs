using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.DeleteProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

public sealed class ProductDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context) =>
        await mediator.Send(new DeleteProductSnapshotCommand(context.Message.ProductId), context.CancellationToken);
}
