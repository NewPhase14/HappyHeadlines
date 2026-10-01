using CoreDomain;

namespace ArticleDatabase.Repository;

public interface IArticleRepository
{
    Task<List<Article>> GetAllArticles();
    Task<List<Article>> GetLatestArticles();
    Task<Article?> GetArticleById(string articleId);
    Task<Article> CreateArticle(Article article);
    Task<Article?> UpdateArticle(Article article);
    Task DeleteArticle(string articleId);
}