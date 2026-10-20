using System.Diagnostics;
using ArticleService.Service;
using CoreDomain;
using CoreDomain.Dtos.ArticleDtos;
using EasyNetQ;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace ArticleService;

public class ArticleHandler(
    IBus bus,
    IServiceScopeFactory scopeFactory, TraceContextPropagator traceContextPropagator) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await bus.PubSub.SubscribeAsync<TracedMessage<CreateArticleDto>>(
            "article-service",
            async tracedMessage =>
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
                        "consume article-published",
                        ActivityKind.Consumer,
                        parentContext.ActivityContext);

               await using var scope = scopeFactory.CreateAsyncScope();

                var articleService =
                    scope.ServiceProvider
                        .GetRequiredService<IArticleService>();

                await articleService.CreateArticle(tracedMessage.Data);
            },
            x => x.WithTopic("article-published"),
            stoppingToken); 

        MonitoringService.log.Information("Subscribed to article-published");

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