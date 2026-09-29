using ArticleService;
using ArticleService.Interface;
using CoreDomain;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TraceContextPropagator>();

builder.Services.AddSingleton<IArticleService, ArticleService.ArticleService>();

builder.Services.AddHttpClient(
    "CommentService",
    client =>
    {
        client.BaseAddress = new Uri("http://comment-service:8080/");
        client.Timeout = TimeSpan.FromSeconds(2);
    });


builder.Services.AddSingleton(
    RabbitHutch.CreateBus(builder.Configuration["AppOptions:RABBITMQ_HOST"]));
builder.Services.AddHostedService<ArticleHandler>();

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