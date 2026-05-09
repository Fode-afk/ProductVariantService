using ProductVariantService.Application.DependencyInjection;
using ProductVariantService.Infrastructure.DependencyInjection;
using ProductVariantService.Api.Grpc.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

//await app.MigrateDatabaseAsync();
//await app.SeedDatabaseAsync();

app.UseHttpsRedirection();

app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();