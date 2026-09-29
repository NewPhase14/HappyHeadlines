using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION"));
IDatabase db = redis.GetDatabase();

db.StringSet("bike:1", "Process 134");
Console.WriteLine(db.StringGet("bike:1"));

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