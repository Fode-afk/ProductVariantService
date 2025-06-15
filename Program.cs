using MongoDB.Driver;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.EventProcessing;
using ProductService.Grpc;
using ProductService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepo, ProductRepo>();

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

var mongoSettings = builder.Configuration
    .GetSection("MongoSettings")
    .Get<MongoSettings>();

var client = new MongoClient(mongoSettings.ConnectionString);
var database = client.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton(database);

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddOpenApi();

var app = builder.Build();

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
