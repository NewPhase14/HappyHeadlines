using CoreDomain;

namespace ArticleCache.Service;

public interface IArticleCacheService
{
    Task<Article?> GetArticleById(string articleId);

    Task RefreshCache();
}