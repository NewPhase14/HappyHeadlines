namespace SubscriberQueue.Interfaces;

public interface ISubscriberQueue
{
    Task SubscribeAsync<T>(string subscription, Action<T> handler, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(string subscription, T message, CancellationToken cancellationToken = default);
    Task UnsubscribeAsync(string subscription, CancellationToken cancellationToken = default);
}