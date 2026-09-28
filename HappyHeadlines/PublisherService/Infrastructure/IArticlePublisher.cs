using CoreDomain;

namespace PublisherService.Infrastructure;

public interface IArticlePublisher
{
    public Task PublishArticle(Article article);
}