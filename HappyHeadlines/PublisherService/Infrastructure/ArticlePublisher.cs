using ArticleQueue.Interfaces;
using CoreDomain.Dtos.ArticleDtos;

namespace PublisherService.Infrastructure;

public class ArticlePublisher(IArticleQueue articleQueue) : IArticlePublisher
{
    public async Task PublishArticle(CreateArticleDto createArticleDto)
    {
        await articleQueue.PublishAsync("article-published", createArticleDto);
        await articleQueue.PublishAsync("newsletter-published", createArticleDto);
    }
}