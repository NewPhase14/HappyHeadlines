using CoreDomain;

namespace ArticleService.Interface;

public interface IArticleService
{
    Task CreateArticle(Article article);
    
    Task<List<Article>> GetLatestArticles();
}