using ProductService.Api.Grpc.V1;
using ProductService.Application.DependencyInjection;
using ProductService.Infrastructure.DependencyInjection;

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