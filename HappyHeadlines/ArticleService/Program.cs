using ArticleDatabase.Repository;
using ArticleService;
using ArticleService.Service;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService.Service.ArticleService>();
builder.Services.AddHostedService<ArticleHandler>();

builder.Services.AddHttpClient(
    "CommentService",
    client =>
    {
        client.BaseAddress = new Uri("http://comment-service:8080/");
        client.Timeout = TimeSpan.FromSeconds(2);
    });

builder.Services.AddSingleton(
    RabbitHutch.CreateBus(builder.Configuration["AppOptions:RABBITMQ_HOST"]));


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