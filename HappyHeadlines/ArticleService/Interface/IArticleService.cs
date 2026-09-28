using CoreDomain;

namespace ArticleService.Interface;

public interface IArticleService
{
    Task<bool> CreateArticle(Article article);
}