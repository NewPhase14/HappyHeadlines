using ArticleQueue.Configuration;
using ArticleQueue.Extensions;
using OpenTelemetry.Context.Propagation;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddArticleQueue(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable(
        "AppOptions__RABBITMQ_HOST");
    options.MessagingProvider = MessagingProvider.RabbitMQ;
});

builder.Services.AddSingleton<TraceContextPropagator>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();