using ArticleQueue.Configuration;
using ArticleQueue.Factories;
using OpenTelemetry.Context.Propagation;
using PublisherService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IArticlePublisher>(new ArticlePublisher(
    RabbitMQFactory.Create(
        new ArticleQueueOptions
        {
            ConnectionString = Environment.GetEnvironmentVariable(
                "AppOptions__RABBITMQ_HOST"),
            MessagingProvider = MessagingProvider.RabbitMQ
        }
    )
));

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