using Microsoft.EntityFrameworkCore;
using ProfanityDatabase;
using ProfanityDatabase.Repository;
using ProfanityService.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProfanityRepository, ProfanityRepository>();
builder.Services.AddScoped<IProfanityService, ProfanityService.Service.ProfanityService>();

builder.Services.AddDbContext<ProfanityContext>(options =>
    options.UseNpgsql(
        builder.Configuration["PROFANITY_DB_CONNECTION"]
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