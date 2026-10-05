using ArticleCache;
using ArticleCache.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ArticleCacheWorker>();

var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection)
);

builder.Services.AddScoped<IArticleCacheService, ArticleCacheService>();

builder.Services.AddHttpClient(
    "ArticleService",
    client =>
    {
        client.BaseAddress = new Uri("http://article-service:8080/");
        client.Timeout = TimeSpan.FromSeconds(2);
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();