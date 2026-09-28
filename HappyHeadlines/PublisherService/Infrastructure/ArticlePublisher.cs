using ArticleQueue.Interfaces;
using CoreDomain;

namespace PublisherService.Infrastructure;

public class ArticlePublisher(IArticleQueue articleQueue) : IArticlePublisher
{
    public async Task PublishArticle(Article article)
    {
        
        if (article.Id == null) return;
        await articleQueue.PublishAsync("article_published", article);
        await articleQueue.PublishAsync("newsletter_published", article);
    }
    
}