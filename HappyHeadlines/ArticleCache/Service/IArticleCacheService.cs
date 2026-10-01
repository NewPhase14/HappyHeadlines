using CoreDomain;

namespace ArticleCache.Service;

public interface IArticleCacheService
{
    Task<Article> GetArticleAsync(string articleId);

    Task<List<Article>> GetArticlesAsync();

    Task GetLatestArticlesAsync();
}