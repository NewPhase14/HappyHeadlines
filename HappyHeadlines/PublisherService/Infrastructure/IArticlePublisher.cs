using CoreDomain.Dtos.ArticleDtos;

namespace PublisherService.Infrastructure;

public interface IArticlePublisher
{
    public Task PublishArticle(CreateArticleDto createArticleDto);
}