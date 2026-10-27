using SubscriberQueue.Interfaces;
using EasyNetQ;

namespace SubscriberQueue.Implementation;

public class RabbitMQAdapter(IBus bus) : ISubscriberQueue
{
    private readonly Dictionary<string, IDisposable> _subscriptions = new();

    public async Task SubscribeAsync<T>(
        string topic,
        Action<T> handler,
        CancellationToken cancellationToken = default)
    {
        if (_subscriptions.ContainsKey(topic))
            throw new ArgumentException(
                $"Already subscribed to topic '{topic}'.");

        var subscriptionHandle =
            await bus.PubSub.SubscribeAsync(
                Guid.NewGuid().ToString(),
                handler,
                x => x.WithTopic(topic),
                cancellationToken);

        _subscriptions.Add(topic, subscriptionHandle);
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Publishing message to {topic}");

        await bus.PubSub.PublishAsync(message, topic, cancellationToken);
    }

    public async Task UnsubscribeAsync(string subscription, CancellationToken cancellationToken = default)
    {
        if (_subscriptions.TryGetValue(subscription, out var subscriptionHandle))
        {
            subscriptionHandle.Dispose();
            _subscriptions.Remove(subscription);
        }

        await Task.CompletedTask;
    }
}