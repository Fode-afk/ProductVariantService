using ProductVariantService.Application.DependencyInjection;
using ProductVariantService.Infrastructure.DependencyInjection;
using ProductVariantService.Api.Grpc.V1;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((ctx, services, config) =>
{
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("ServiceName", "ProductVariantService")
        .WriteTo.Async(a => a.Console(new JsonFormatter()),
            bufferSize: 10000,
            blockWhenFull: false)
        .WriteTo.Async(a => a.OpenTelemetry(opts =>
        {
            opts.Endpoint = builder.Configuration.GetConnectionString("OtlpEndpoint")
                ?? throw new InvalidOperationException("OtlpEndpoint is not configured");
            opts.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = "ProductVariantService"
            };
        }),
            bufferSize: 10000,
            blockWhenFull: false);
});

var app = builder.Build();

//await app.MigrateDatabaseAsync();
//await app.SeedDatabaseAsync();

app.UseHttpsRedirection();

app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();