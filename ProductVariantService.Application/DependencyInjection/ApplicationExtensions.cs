using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductVariantService.Application.DependencyInjection;

namespace ProductVariantService.Application.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddValidators()
            .AddMediatR()
            .AddTimeProvider();

    private static IServiceCollection AddValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddTimeProvider(this IServiceCollection services) =>
        services.AddSingleton(TimeProvider.System);
}
