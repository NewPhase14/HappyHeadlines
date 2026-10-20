using System.Diagnostics;
using CoreDomain;
using CoreDomain.Dtos.ArticleDtos;
using EasyNetQ;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace NewsletterService;

public class ArticleHandler(IBus bus, TraceContextPropagator traceContextPropagator) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await bus.PubSub.SubscribeAsync<TracedMessage<CreateArticleDto>>(
            "newsletter-service",
            tracedMessage =>
            {
                var parentContext = traceContextPropagator.Extract(
                    default,
                    tracedMessage.Headers,
                    (headers, key) =>
                    {
                        return new List<string>(new[] { headers.ContainsKey(key) ? headers[key].ToString() : string.Empty });
                    });

                Baggage.Current = parentContext.Baggage;

                using var activity =
                    MonitoringService.ActivitySource.StartActivity(
                        "consume newsletter-published",
                        ActivityKind.Consumer,
                        parentContext.ActivityContext);
            },
            x => x.WithTopic("newsletter-published"),
            stoppingToken);

        MonitoringService.log.Information("Subscribed to newsletter-published");

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

        MonitoringService.log.Error("Message handler is stopping...");
    }
}