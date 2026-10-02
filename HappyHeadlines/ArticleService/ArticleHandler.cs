using ArticleService.Service;
using CoreDomain.Dtos.ArticleDtos;
using EasyNetQ;
using Monitoring;

namespace ArticleService;

public class ArticleHandler(
    IBus bus,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("MessageHandler starting...");
        Console.WriteLine("Connected to RabbitMQ");

        Console.WriteLine(
            $"Creating RabbitMQ subscription. PID={Environment.ProcessId}");

        await bus.PubSub.SubscribeAsync<CreateArticleDto>(
            "article-service",
             createArticleDto =>
            {
                MonitoringService.ActivitySource.StartActivity();

                Console.WriteLine(
                    $"[{Environment.MachineName}] Received message: " +
                    $"{createArticleDto.Title} - {createArticleDto.Content}");

                using var scope = scopeFactory.CreateAsyncScope();

                var articleService =
                    scope.ServiceProvider
                        .GetRequiredService<IArticleService>();

                articleService.CreateArticle(createArticleDto);
            },
            x => x.WithTopic("article-published"),
            stoppingToken); 

        Console.WriteLine("Subscribed to article-published");

        try
        {
             await Task.Delay(
                 Timeout.Infinite,
                 stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Application is shutting down
        }

        Console.WriteLine("Message handler is stopping...");
    }
}