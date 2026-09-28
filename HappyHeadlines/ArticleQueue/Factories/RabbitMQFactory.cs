using ArticleQueues.Configuration;
using ArticleQueues.Implementation;
using EasyNetQ;

namespace ArticleQueues.Factories;

public static class RabbitMQFactory {
    public static RabbitMQAdapter Create(ArticleQueueOptions options) {
        IBus bus = RabbitHutch.CreateBus(options.ConnectionString + ";publisherConfirms=true;timeout=10");
        return new RabbitMQAdapter(bus);
    }
}