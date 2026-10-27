namespace SubscriberQueue.Configuration;

public enum MessagingProvider
{
    RabbitMQ,
    Kafka
}

public class SubscriberQueueOptions
{
    public MessagingProvider MessagingProvider { get; set; } = MessagingProvider.RabbitMQ;
    public string ConnectionString { get; set; } = "";
}