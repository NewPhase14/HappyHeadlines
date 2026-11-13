using CommentDatabase;
using CommentDatabase.Repository;
using CommentService.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService.Service.CommentService>();

builder.Services.AddDbContext<CommentContext>(options =>
    options.UseNpgsql(
        builder.Configuration["COMMENT_DB_CONNECTION"]
    ));

builder.Services.AddHttpClient(
    "ProfanityService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://profanity-service:8080/");

        client.Timeout = TimeSpan.FromSeconds(2);
    })
    .AddResilienceHandler("ProfanityCircuitBreaker", pipeline =>
    {
        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(10),
            MinimumThroughput = 4,
            BreakDuration = TimeSpan.FromSeconds(15)
        });
    });

builder.Services.AddHttpClient(
    "CommentCacheService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://comment-cache-service:8080/");

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