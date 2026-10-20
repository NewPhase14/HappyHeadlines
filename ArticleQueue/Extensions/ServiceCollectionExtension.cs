using ArticleQueue.Configuration;
using ArticleQueue.Factories;
using ArticleQueue.Interfaces;
using OpenTelemetry.Context.Propagation;

namespace ArticleQueue.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddArticleQueue(
        this IServiceCollection services,
        Action<ArticleQueueOptions> configureOptions
    )
    {
        var options = new ArticleQueueOptions();
        configureOptions(options);

        switch (options.MessagingProvider)
        {
            case MessagingProvider.RabbitMQ:

                services.AddSingleton<IArticleQueue>(sp =>
                {
                    var traceContextPropagator =
                        sp.GetRequiredService<TraceContextPropagator>();

                    return RabbitMQFactory.Create(
                        options,
                        traceContextPropagator);
                });

                break;

            default:
                throw new ArgumentException(
                    "Unsupported messaging provider");
        }

        return services;
    }

    public static IServiceCollection AddMessageHandler(
        this IServiceCollection services,
        Action<ArticleQueueOptions> configureClientOptions
    )
    {
        services.AddArticleQueue(configureClientOptions);

        return services;
    }
}