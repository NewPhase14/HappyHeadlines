using ArticleQueue.Interfaces;
using CoreDomain;

namespace PublisherService.Infrastructure;

public class ArticlePublisher(IArticleQueue articleQueue) : IArticlePublisher
{
    public async Task PublishArticle(Article article)
    {
        await articleQueue.PublishAsync("article-published", article);
        await articleQueue.PublishAsync("newsletter-published", article);
    }
    
}