using migApp.Shared.Logging;
using ProductVariantService.Api.Grpc.V1;
using ProductVariantService.Application.DependencyInjection;
using ProductVariantService.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseMigAppSerilog("ProductVariantService");

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

//await app.MigrateDatabaseAsync();
//await app.SeedDatabaseAsync();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();