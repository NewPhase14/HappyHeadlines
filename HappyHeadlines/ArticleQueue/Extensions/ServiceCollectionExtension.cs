using ArticleQueue.Interfaces;
using ArticleQueues.Configuration;
using ArticleQueues.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace ArticleQueues.Extensions;

public static class ServiceCollectionExtension {
    
    public static IServiceCollection AddArticleQueue(
        this IServiceCollection services,
        Action<ArticleQueueOptions> configureOptions
    ){
        ArticleQueueOptions options = new ArticleQueueOptions();
        configureOptions(options);

        IArticleQueue? messagingClient;
        switch(options.MessagingProvider){
            case MessagingProvider.RabbitMQ:
                messagingClient = RabbitMQFactory.Create(options);
                services.AddSingleton<IArticleQueue>(
                    sp => messagingClient
                );
                break;
            default:
                throw new ArgumentException("");
        }
        return services;
    }
    
    public static IServiceCollection AddMessageHandler(
        this IServiceCollection services,
        Action<ArticleQueueOptions> configureClientOptions
    ){
        services.AddArticleQueue(configureClientOptions);
        
        return services;
    }
}