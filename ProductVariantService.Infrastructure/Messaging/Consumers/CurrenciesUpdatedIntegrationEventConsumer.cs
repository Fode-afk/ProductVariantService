using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ExchangeRates;
using ProductVariantService.Application.Interfaces.Services;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

public sealed class CurrenciesUpdatedIntegrationEventConsumer(IExchangeRateService exchangeRateService) : IConsumer<CurrenciesUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CurrenciesUpdatedIntegrationEvent> context) =>
        await exchangeRateService.UpdateExcahngeRatesAsync(context.Message.Rates, context.CancellationToken);
}
