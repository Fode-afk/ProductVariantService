using migApp.Shared.MsgBus;
using MongoDB.Driver;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.Data.Caching;
using ProductService.EventProcessing;
using ProductService.Grpc;
using ProductService.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICacheRepo, CacheRepo>();

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis")!
    );
    redisConfig.AbortOnConnectFail = false;
    redisConfig.ReconnectRetryPolicy = new ExponentialRetry(5000);

    return ConnectionMultiplexer.Connect(redisConfig);
});

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

var mongoSettings = builder.Configuration
    .GetSection("MongoSettings")
    .Get<MongoSettings>();

if (mongoSettings?.ConnectionString == null)
    throw new InvalidOperationException("MongoDB connection string is not configured");

var client = new MongoClient(mongoSettings.ConnectionString);
var database = client.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton(database);

var mongoInitializer = new MongoDbInitializer(database);
await mongoInitializer.InitializeAsync();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddHostedService<MessageBusClientInitializer>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<ILogger>(provider =>
{
    var logPath = Path.Combine("Logs", $"{DateTime.UtcNow:yyyy-MM-dd HH-mm-ss}.log");
    return new Logger(logPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcServer>();

//var products = await new TestingUtils().ParseProductsAsync(builder.Configuration, 4);
//new TestingUtils().Proccess(products, builder.Services.BuildServiceProvider().GetRequiredService<IProductRepo>());

app.Run();
