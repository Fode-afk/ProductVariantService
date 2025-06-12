using MongoDB.Driver;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.Grpc;
using ProductService.Settings;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adds services to the dependency injection container.
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Registers the product repository service with scoped lifetime.
/// </summary>
builder.Services.AddScoped<IProductRepo, ProductRepo>();

/// <summary>
/// Adds gRPC services to the container.
/// </summary>
builder.Services.AddGrpc();

/// <summary>
/// Registers AutoMapper with all assemblies in the current domain.
/// </summary>
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

/// <summary>
/// Configures MongoDB settings by binding the configuration section "MongoSettings".
/// </summary>
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

var mongoSettings = builder.Configuration
    .GetSection("MongoSettings")
    .Get<MongoSettings>();

/// <summary>
/// Creates a MongoClient and registers the Mongo database instance as a singleton service.
/// </summary>
var client = new MongoClient(mongoSettings.ConnectionString);
var database = client.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton(database);

/// <summary>
/// Registers a hosted background service that listens for messages from the message bus.
/// </summary>
/// <remarks>
/// The <see cref="MessageBusSubscriber" /> is started when the application starts and stops when the application shuts down.
/// Typically used for asynchronous integration via pub/sub messaging.
/// </remarks>
builder.Services.AddHostedService<MessageBusSubscriber>();

/// <summary>
/// Registers OpenAPI (Swagger) services.
/// </summary>
builder.Services.AddOpenApi();

var app = builder.Build();

/// <summary>
/// Configures the HTTP request pipeline.
/// Enables OpenAPI UI in development environment.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/// <summary>
/// Maps the gRPC server service to the request pipeline.
/// </summary>
app.MapGrpcService<GrpcServer>();

/// <summary>
/// Maps the products.proto file to an HTTP GET endpoint.
/// This allows clients to fetch the protobuf contract directly.
/// </summary>
app.MapGet("/protos/products.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/products.proto"));
});

app.Run();
