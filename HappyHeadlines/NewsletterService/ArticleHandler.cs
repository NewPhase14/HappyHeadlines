using CoreDomain;
using EasyNetQ;

namespace NewsletterService;

public class ArticleHandler(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("MessageHandler starting...");

        Console.WriteLine("Connected to RabbitMQ");

        await bus.PubSub.SubscribeAsync<Article>(
            "newsletter-published",
            msg =>
            {
                Console.WriteLine(
                    $"Received message: {msg.Id} - {msg.Title}");
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