using ArticleQueue.Configuration;
using ArticleQueue.Implementation;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;

namespace ArticleQueue.Factories;

public static class RabbitMQFactory
{
    public static RabbitMQAdapter Create(ArticleQueueOptions options, TraceContextPropagator traceContextPropagator)
    {
        var bus = RabbitHutch.CreateBus(options.ConnectionString + ";publisherConfirms=true;timeout=10");
        return new RabbitMQAdapter(bus, traceContextPropagator);
    }
}