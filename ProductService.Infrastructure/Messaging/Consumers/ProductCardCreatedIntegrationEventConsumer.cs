using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;

namespace ProductService.Infrastructure.Messaging.Consumers;

public sealed class ProductCardCreatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductCardCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCardCreatedIntegrationEvent> context) => 
        await mediator.Send(new (), context.CancellationToken);
}
