using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using migApp.Shared.Behaviours;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Grpc;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Application.Interfaces.Metrics;
using ProductVariantService.Infrastructure.BackgroundServices;
using ProductVariantService.Infrastructure.Behaviours;
using ProductVariantService.Infrastructure.Data;
using ProductVariantService.Infrastructure.DomainEvents;
using ProductVariantService.Infrastructure.Messaging;
using ProductVariantService.Infrastructure.Messaging.Consumers;
using ProductVariantService.Infrastructure.Messaging.IntegrationEvents;
using ProductVariantService.Infrastructure.Observability;
using RabbitMQ.Client;

namespace ProductVariantService.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) => 
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddGrpcServices()
            .AddHealth(configuration)
            .AddMessaging(configuration)
            .AddIntegrationEventHandlers()
            .AddObservability(configuration)
            .AddBehaviours();

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services
            .AddOptions<SoftDeletedProductVariantsCleanupOptions>()
            .Bind(configuration.GetSection(
                SoftDeletedProductVariantsCleanupOptions.SectionName))
            .Validate(
                options => options.Interval > TimeSpan.Zero,
                $"{nameof(SoftDeletedProductVariantsCleanupOptions.Interval)} must be greater than 0.")
            .Validate(
                options => options.RetentionPeriod > TimeSpan.Zero,
                $"{nameof(SoftDeletedProductVariantsCleanupOptions.RetentionPeriod)} must be greater than 0.")
            .Validate(
                options => options.BatchSize > 0,
                $"{nameof(SoftDeletedProductVariantsCleanupOptions.BatchSize)} must be greater than 0.")
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<SoftDeletedProductVariantsCleanupOptions>>().Value);

        services.AddHostedService<SoftDeletedProductVariantsCleanupService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.ProductVariantWrite)));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    private static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc(options => options.Interceptors.Add<GrpcExceptionInterceptor>());
        return services;
    }

    private static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        var rabbitMq = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()
                ?? throw new InvalidOperationException("RabbitMQ configuration is missing.");

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is missing");

        services.AddGrpcHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
            .AddSqlServer(
                connectionString: databaseConnectionString,
                name: "mssql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRabbitMQ(
                factory: async sp =>
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rabbitMq.Host,
                        Port = rabbitMq.Port,
                        VirtualHost = rabbitMq.VirtualHost,
                        UserName = rabbitMq.Username,
                        Password = rabbitMq.Password
                    };
                    return await factory.CreateConnectionAsync();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services
           .AddOptions<RabbitMqOptions>()
           .BindConfiguration(RabbitMqOptions.SectionName)
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Host),
               "RabbitMQ:Host is required.")
           .Validate(
               options => options.Port > 0,
               "RabbitMQ:Port must be greater than zero.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Username),
               "RabbitMQ:Username is required.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Password),
               "RabbitMQ:Password is required.")
           .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("product-variant-service", false));

            x.AddConsumersFromNamespaceContaining<ConsumersAssemblyMarker>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                cfg.Host(options.Host,
                    options.Port,
                    options.VirtualHost,
                    host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
                    });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssemblies(typeof(IntegrationEventsAssemblyMarker).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IPreCommitDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

    private static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var otlpEndpoint = configuration["Otlp:Endpoint"]
            ?? throw new InvalidOperationException("OtlpEndpoint is not configured");

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("ProductVariantService"))
            .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otlpEndpoint))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(opts =>
                    opts.Filter = ctx =>
                        !ctx.Request.Path.StartsWithSegments("/health"))
                .AddHttpClientInstrumentation()             
                .AddSource("MassTransit")
                .AddSource("ProductVariantService"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("MassTransit")
                .AddMeter(ProductVariantServiceMetrics.MeterName));

        services.AddSingleton<IProductVariantMetrics, ProductVariantServiceMetrics>();
        services.AddHostedService<ActiveVariantsMetricCollector>();

        return services;
    }

    private static IServiceCollection AddBehaviours(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}