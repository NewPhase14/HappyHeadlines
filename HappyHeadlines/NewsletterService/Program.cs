using CoreDomain;
using EasyNetQ;
using NewsletterService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(
    RabbitHutch.CreateBus(builder.Configuration["AppOptions:RABBITMQ_HOST"]));
builder.Services.AddHostedService<ArticleHandler>();

builder.Services.AddHttpClient(
    "ArticleService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://article-service:8080/");

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