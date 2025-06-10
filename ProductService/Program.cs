using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepo, ProductRepo>();

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcServer>();

app.MapGet("/protos/products.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/products.proto"));
});

app.Run();
