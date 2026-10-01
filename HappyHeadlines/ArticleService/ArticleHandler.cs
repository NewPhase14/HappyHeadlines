using ArticleService.Service;
using CoreDomain.Dtos.ArticleDtos;
using EasyNetQ;

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
            "article-published",
            async createArticleDto =>
            {
                Console.WriteLine(
                    $"[{Environment.MachineName}] Received message: " +
                    $"{createArticleDto.Title} - {createArticleDto.Content}");

                await using var scope =
                    scopeFactory.CreateAsyncScope();

                var articleService =
                    scope.ServiceProvider
                        .GetRequiredService<IArticleService>();

                await articleService.CreateArticle(createArticleDto);
            },
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