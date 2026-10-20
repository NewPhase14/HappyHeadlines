using ArticleDatabase.Repository;
using ArticleService;
using ArticleService.Service;
using EasyNetQ;
using Monitoring;
using OpenTelemetry.Context.Propagation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

MonitoringService.Configure("ArticleService");

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService.Service.ArticleService>();
builder.Services.AddHostedService<ArticleHandler>();

builder.Services.AddSingleton<TraceContextPropagator>();

builder.Services.AddSingleton(
    RabbitHutch.CreateBus(builder.Configuration["AppOptions:RABBITMQ_HOST"]));

builder.Services.AddHttpClient(
    "ArticleCacheService",
    client =>
    {
        client.BaseAddress = new Uri("http://article-cache-service:8080/");
        client.Timeout = TimeSpan.FromSeconds(2);
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