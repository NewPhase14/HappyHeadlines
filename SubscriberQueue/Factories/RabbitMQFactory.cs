using SubscriberQueue.Configuration;
using SubscriberQueue.Implementation;
using EasyNetQ;

namespace SubscriberQueue.Factories;

public static class RabbitMQFactory
{
    public static RabbitMQAdapter Create(SubscriberQueueOptions options)
    {
        var bus = RabbitHutch.CreateBus(options.ConnectionString + ";publisherConfirms=true;timeout=10");
        return new RabbitMQAdapter(bus);
    }
}