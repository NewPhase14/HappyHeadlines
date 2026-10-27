using CoreDomain;
using EasyNetQ;

namespace NewsletterService;

public class SubscriberHandler(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("MessageHandler starting...");

        Console.WriteLine("Connected to RabbitMQ");

        Console.WriteLine(
            $"Creating RabbitMQ subscription. PID={Environment.ProcessId}");

        await bus.PubSub.SubscribeAsync<Subscriber>(
            "subscriber-created",
            subscriber =>
            {
                Console.WriteLine(
                    $"Welcome mail has been sent to {subscriber.Email}");
            },
            stoppingToken);

        Console.WriteLine("Subscribed to subscriber-created");

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