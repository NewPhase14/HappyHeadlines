using DraftDatabase;
using DraftDatabase.Repository;
using DraftService.Service;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Context.Propagation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDraftRepository, DraftRepository>();
builder.Services.AddScoped<IDraftService, DraftService.Service.DraftService>();


builder.Services.AddDbContext<DraftContext>(options =>
    options.UseNpgsql(
        builder.Configuration["DRAFT_DB_CONNECTION"]
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