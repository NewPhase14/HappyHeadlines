using ArticleQueue.Configuration;
using ArticleQueue.Factories;
using Monitoring;
using OpenTelemetry.Context.Propagation;
using PublisherService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

MonitoringService.Configure("PublisherService");

builder.Services.AddSingleton<TraceContextPropagator>();

builder.Services.AddSingleton<IArticlePublisher>(sp =>
{
    var traceContextPropagator =
        sp.GetRequiredService<TraceContextPropagator>();

    var articleQueue = RabbitMQFactory.Create(
        new ArticleQueueOptions
        {
            ConnectionString = Environment.GetEnvironmentVariable(
                "AppOptions__RABBITMQ_HOST"),

            MessagingProvider = MessagingProvider.RabbitMQ
        },
        traceContextPropagator);

    return new ArticlePublisher(articleQueue);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();