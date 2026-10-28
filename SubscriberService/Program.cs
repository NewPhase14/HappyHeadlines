using FeatureHubSDK;
using Microsoft.EntityFrameworkCore;
using SubscriberDatabase;
using SubscriberDatabase.Repository;
using SubscriberQueue.Configuration;
using SubscriberQueue.Factories;
using SubscriberQueue.Interfaces;
using SubscriberService.Service;

var builder = WebApplication.CreateBuilder(args);

var featureHubConfig = new EdgeFeatureHubConfig(
    builder.Configuration["FEATUREHUB_URL"],
    builder.Configuration["FEATUREHUB_API_KEY"]
);
builder.Services.AddSingleton<IFeatureHubConfig>(featureHubConfig);
await featureHubConfig.Init();

var featureHubContext = await featureHubConfig.NewContext().Build();
builder.Services.AddSingleton(featureHubContext);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
builder.Services.AddScoped<ISubscriberService, SubscriberService.Service.SubscriberService>();

builder.Services.AddDbContext<SubscriberContext>(options => 
    options.UseNpgsql(builder.Configuration["SUBSCRIBER_DB_CONNECTION"]
    ));

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