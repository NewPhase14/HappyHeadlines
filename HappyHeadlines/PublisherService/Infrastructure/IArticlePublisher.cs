using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace PublisherService.Infrastructure;

public interface IArticlePublisher
{
    public Task PublishArticle(Article article);
}