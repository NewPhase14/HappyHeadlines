using ArticleQueue.Interfaces;
using EasyNetQ;

namespace ArticleQueue.Implementation;

public class RabbitMQAdapter : IArticleQueue
{
    private readonly IBus _bus;
    private readonly Dictionary<string, IDisposable> _subscriptions = new();

    public RabbitMQAdapter(IBus bus)
    {
        _bus = bus;
    }

    public async Task SubscribeAsync<T>(
        string topic,
        Action<T> handler,
        CancellationToken cancellationToken = default)
    {
        if (_subscriptions.ContainsKey(topic))
            throw new ArgumentException(
                $"Already subscribed to topic '{topic}'.");

        var subscriptionHandle =
            await _bus.PubSub.SubscribeAsync(
                Guid.NewGuid().ToString(),
                handler,
                x => x.WithTopic(topic),
                cancellationToken);

        _subscriptions.Add(topic, subscriptionHandle);
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Publishing message to {topic}");

        await _bus.PubSub.PublishAsync(message, topic, cancellationToken);
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