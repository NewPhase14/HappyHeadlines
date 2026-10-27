using SubscriberDatabase.Repository;
using SubscriberQueue.Configuration;
using SubscriberQueue.Factories;
using SubscriberQueue.Interfaces;
using SubscriberService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISubscriberService, SubscriberService.Service.SubscriberService>();
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();

builder.Services.AddSingleton<ISubscriberQueue>(
    RabbitMQFactory.Create(
        new SubscriberQueueOptions
        {
            ConnectionString = builder.Configuration["AppOptions:RABBITMQ_HOST"],
            MessagingProvider = MessagingProvider.RabbitMQ
        }
    )
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

app.MapControllers();

app.Run();