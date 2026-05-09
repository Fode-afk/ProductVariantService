using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

//public sealed class PriceCreatedIntegrationEventConsumer(IMediator mediator) : //IConsumer<PriceCreatedIntegrationEvent>
//{
//    public async Task Consume(ConsumeContext<PriceCreatedIntegrationEvent> context) =>
//        await mediator.Send(new UpdateProductPriceSnapshotCommand(
//            context.Message.ProductId,
//            context.Message.VendorId,
//            context.Message.Price,
//            OldPrice: null), context.CancellationToken);
//}
