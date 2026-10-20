using CoreDomain.Dtos.ArticleDtos;

namespace ArticleService.Service;

public interface IArticleService
{
    Task<ArticleResponseDto> CreateArticle(CreateArticleDto createArticleDto);
    Task<List<ArticleResponseDto>> GetArticlesFromLast14Days();
    Task<ArticleResponseDto> GetArticleById(string articleId);
    Task<List<ArticleResponseDto>> GetAllArticles();
    Task<ArticleResponseDto> UpdateArticle(UpdateArticleDto updateArticleDto);
    Task DeleteArticle(string articleId);
    Task<ArticleResponseDto> GetRandomArticle();
}