using ArticleDatabase.Repository;
using CoreDomain;
using CoreDomain.Dtos.ArticleDtos;

namespace ArticleService.Service;

public class ArticleService(IArticleRepository articleRepository, IHttpClientFactory httpClientFactory)
    : IArticleService
{
    
    private readonly HttpClient cacheClient = httpClientFactory.CreateClient("ArticleCacheService");

    public async Task<ArticleResponseDto> CreateArticle(CreateArticleDto createArticleDto)
    {
        var article = new Article
        {
            Id = Guid.NewGuid().ToString(),
            Title = createArticleDto.Title,
            Content = createArticleDto.Content,
            Continent = createArticleDto.Continent,
            Draft = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdArticle = await articleRepository.CreateArticle(article);

        return new ArticleResponseDto
        {
            Id = createdArticle.Id,
            Title = createdArticle.Title,
            Content = createdArticle.Content,
            Continent = createdArticle.Continent,
            Draft = createdArticle.Draft,
            CreatedAt = createdArticle.CreatedAt
        };
    }

    public async Task<List<ArticleResponseDto>> GetArticlesFromLast14Days()
    {
        var articles = await articleRepository.GetArticlesFromLast14Days();

        return articles.Select(a => new ArticleResponseDto
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            Continent = a.Continent,
            Draft = a.Draft,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<ArticleResponseDto> GetArticleById(string articleId)
    {
        var response = await cacheClient.GetAsync($"ArticleCache/getArticleById/{articleId}");
        
        Article? article = null;
        
        if (response.IsSuccessStatusCode)
        {
            article = await response.Content.ReadFromJsonAsync<Article>();
        }
        
        else
        {
            Console.WriteLine("Article not found in cache");
            
            article = await articleRepository.GetArticleById(articleId);
            
            if (article == null) throw new ArgumentException("No article found with the given ID.");
        }
        
        return new ArticleResponseDto
        {
            Id = article!.Id,
            Title = article.Title,
            Content = article.Content,
            Continent = article.Continent,
            Draft = article.Draft,
            CreatedAt = article.CreatedAt,
        };
    }

    public async Task<List<ArticleResponseDto>> GetAllArticles()
    {
        var articles = await articleRepository.GetAllArticles();
        if (articles.Count == 0) throw new InvalidOperationException("No articles found.");
        
        return articles.Select(a => new ArticleResponseDto
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            Continent = a.Continent,
            Draft = a.Draft,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<ArticleResponseDto> UpdateArticle(UpdateArticleDto updateArticleDto)
    {
        var articleToBeUpdated = new Article
        {
            Id = updateArticleDto.Id,
            Title = updateArticleDto.Title,
            Content = updateArticleDto.Content,
            Continent = updateArticleDto.Continent,
            Draft = updateArticleDto.Draft
        };

        var updatedArticle = await articleRepository.UpdateArticle(articleToBeUpdated);

        if (updatedArticle == null) throw new ArgumentException("No article found with the given ID.");

        return new ArticleResponseDto
        {
            Id = updatedArticle.Id,
            Title = updatedArticle.Title,
            Content = updatedArticle.Content,
            Continent = updatedArticle.Continent,
            Draft = updatedArticle.Draft,
            CreatedAt = updatedArticle.CreatedAt
        };
    }

    public async Task DeleteArticle(string articleId)
    {
        await articleRepository.DeleteArticle(articleId);
    }
    
    public async Task<ArticleResponseDto> GetLatestArticle()
    {
        var latestArticle = await articleRepository.GetLatestArticle();
        if (latestArticle == null) throw new InvalidOperationException("No articles found.");

        return new ArticleResponseDto
        {
            Id = latestArticle.Id,
            Title = latestArticle.Title,
            Content = latestArticle.Content,
            Continent = latestArticle.Continent,
            Draft = latestArticle.Draft,
            CreatedAt = latestArticle.CreatedAt
        };
    }
    
    
}