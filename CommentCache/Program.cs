using CommentCache.Service;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICommentCacheService, CommentCacheService>();

builder.Services.AddHttpClient(
    "CommentService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://comment-service:8080/");

        client.Timeout = TimeSpan.FromSeconds(2);
    });


var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapMetrics();

app.MapControllers();

app.Run();