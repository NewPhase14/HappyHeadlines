using System.Diagnostics;
using ArticleQueue.Interfaces;
using CoreDomain;
using EasyNetQ;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace ArticleQueue.Implementation;

public class RabbitMQAdapter(IBus bus, TraceContextPropagator traceContextPropagator) : IArticleQueue
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
        using var activity =
            MonitoringService.ActivitySource.StartActivity(
                $"publish {topic}",
                ActivityKind.Producer);

        var tracedMessage = new TracedMessage<T>
        {
            Data = message
        };

        var context = new PropagationContext(
            activity?.Context ?? Activity.Current?.Context ?? default,
            Baggage.Current);

        traceContextPropagator.Inject(
            context,
            tracedMessage.Headers,
            (headers, key, value) =>
            {
                headers[key] = value;
            });

        await bus.PubSub.PublishAsync(
            tracedMessage,
            topic,
            cancellationToken);
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