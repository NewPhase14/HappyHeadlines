using CoreDomain.Dtos.ArticleDtos;
using EasyNetQ;

namespace NewsletterService;

public class ArticleHandler(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("MessageHandler starting...");

        Console.WriteLine("Connected to RabbitMQ");

        Console.WriteLine(
            $"Creating RabbitMQ subscription. PID={Environment.ProcessId}");

        await bus.PubSub.SubscribeAsync<CreateArticleDto>(
            "newsletter-published",
            createArticleDto =>
            {
                Console.WriteLine(
                    $"[{Environment.MachineName}] Received message: " +
                    $"{createArticleDto.Title} - {createArticleDto.Content}");
            },
            stoppingToken);

        Console.WriteLine("Subscribed to newsletter-published");

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