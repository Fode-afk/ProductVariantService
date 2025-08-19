using MongoDB.Driver;
using ProductService.AsyncDataServices;
using ProductService.Data;
using ProductService.EventProcessing;
using ProductService.Grpc;
using ProductService.Protos;
using ProductService.Settings;
using ReviewService.Protos;

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

builder.Services.AddGrpcClient<GrpcReviews.GrpcReviewsClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcReview"]);
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

app.MapGet("/protos/products.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/products.proto"));
});


//var products = await new TestingUtils().ParseProductsAsync(builder.Configuration, 4);
//new TestingUtils().Proccess(products, builder.Services.BuildServiceProvider().GetRequiredService<IProductRepo>());

app.Run();
