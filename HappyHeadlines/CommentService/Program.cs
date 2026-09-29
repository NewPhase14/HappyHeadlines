using CommentService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICommentService, CommentService.Service.CommentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(
    "ProfanityService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://profanity-service:8080/");

        client.Timeout = TimeSpan.FromSeconds(2);
    });

builder.Services.AddHttpClient(
    "CommentCacheService",
    client =>
    {
        client.BaseAddress =
            new Uri("http://comment-cache-service:8080/");

        client.Timeout = TimeSpan.FromSeconds(2);
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();