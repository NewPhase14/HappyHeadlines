using ArticleService.Interface;
using CoreDomain;
using EasyNetQ;

namespace ArticleService;

public class ArticleHandler(IBus bus, IArticleService articleService) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("MessageHandler starting...");

        Console.WriteLine("Connected to RabbitMQ");

        await bus.PubSub.SubscribeAsync<Article>(
            "article-published",
             msg =>
            {
                Console.WriteLine($"Received article with ID: {msg.Id} and Title: {msg.Title}");
                articleService.CreateArticle(msg);
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