using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Vendors;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

//public sealed class VendorCreatedIntegrationEventConsumer(IMediator mediator) : //IConsumer<VendorCreatedIntegrationEvent>
//{
//    public async Task Consume(ConsumeContext<VendorCreatedIntegrationEvent> context) =>
//        await mediator.Send(new AddVendorSnapshotCommand(
//            context.Message.VendorId, 
//            context.Message.Status,
//            context.Message.IsVerified), context.CancellationToken);
//}
