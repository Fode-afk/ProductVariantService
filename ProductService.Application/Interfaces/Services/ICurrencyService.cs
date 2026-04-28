namespace ProductService.Application.Interfaces.Services;

public interface ICurrencyService
{
    Task<decimal?> GetExchangeRateAsync(
       string sourceCurrency,
       string targetCurrency,
       CancellationToken cancellationToken = default);
}