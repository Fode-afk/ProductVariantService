using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using migApp.Shared.Grpc;
using Polly;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Primitives;
using ProductVariantService.Infrastructure.Data;
using ProductVariantService.Infrastructure.DependencyInjection;
using ProductVariantService.Infrastructure.DomainEvents;
using ProductVariantService.Infrastructure.Messaging.Consumers;
using ProductVariantService.Infrastructure.Messaging.IntegrationEvents;
using RabbitMQ.Client;

namespace ProductVariantService.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) => 
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddGrpc(configuration)
            .AddHealthChecks(configuration)
            .AddMassTransit(configuration)
            .AddIntegrationEventHandlers();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.ProductVariantWrite);
            }));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    private static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("Database")!,
                name: "mssql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRabbitMQ(
                factory: sp =>
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["RabbitMQ:Host"]!,
                        Port = int.Parse(configuration["RabbitMQ:Port"]!)
                    };
                    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumersFromNamespaceContaining<ConsumersAssemblyMarker>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"]!, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(IntegrationEventsAssemblyMarker).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IPreCommitDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
