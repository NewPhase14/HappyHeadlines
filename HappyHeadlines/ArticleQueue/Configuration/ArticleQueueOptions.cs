namespace ArticleQueue.Configuration;

public enum MessagingProvider
{
    RabbitMQ,
    Kafka
}

public class ArticleQueueOptions
{
    public MessagingProvider MessagingProvider { get; set; } = MessagingProvider.RabbitMQ;
    public string ConnectionString { get; set; } = "";
}