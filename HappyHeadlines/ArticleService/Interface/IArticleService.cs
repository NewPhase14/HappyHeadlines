using CoreDomain;

namespace ArticleService.Interface;

public interface IArticleService
{
    Task CreateArticle(Article article);
    Task<List<Article>> GetLatestArticles();
    Task<Article> GetArticleById(string articleId);
    Task<List<Article>> GetArticles();
    Task UpdateArticle(string id, Article article);
    Task DeleteArticle(string articleId);
    Task<Article> GetRandomArticle();
}