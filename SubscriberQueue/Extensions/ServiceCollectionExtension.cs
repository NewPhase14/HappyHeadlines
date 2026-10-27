using SubscriberQueue.Configuration;
using SubscriberQueue.Factories;
using SubscriberQueue.Interfaces;

namespace SubscriberQueue.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSubscriberQueue(
        this IServiceCollection services,
        Action<SubscriberQueueOptions> configureOptions
    )
    {
        var options = new SubscriberQueueOptions();
        configureOptions(options);

        ISubscriberQueue? messagingClient;
        switch (options.MessagingProvider)
        {
            case MessagingProvider.RabbitMQ:
                messagingClient = RabbitMQFactory.Create(options);
                services.AddSingleton<ISubscriberQueue>(sp => messagingClient
                );
                break;
            default:
                throw new ArgumentException("");
        }

        return services;
    }

    public static IServiceCollection AddMessageHandler(
        this IServiceCollection services,
        Action<SubscriberQueueOptions> configureClientOptions
    )
    {
        services.AddSubscriberQueue(configureClientOptions);

        return services;
    }
}