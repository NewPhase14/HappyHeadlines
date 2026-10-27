using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SubscriberDatabase;
using SubscriberDatabase.Repository;

var builder = WebApplication.CreateBuilder();

builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();

builder.Services.AddDbContext<SubscriberContext>(options => 
    options.UseNpgsql(builder.Configuration["SUBSCRIBER_DB_CONNECTION"]
    ));

var app = builder.Build();

app.Run();
