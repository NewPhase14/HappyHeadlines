using ArticleDatabase.Repository;
using CoreDomain;
using CoreDomain.Dtos.ArticleDtos;
using CoreDomain.Dtos.CommentDtos;

namespace ArticleService.Service;

public class ArticleService(IHttpClientFactory httpClientFactory, IArticleRepository articleRepository)
    : IArticleService
{
    private readonly HttpClient _commentClient = httpClientFactory.CreateClient("CommentService");

    public async Task<ArticleResponseDto> CreateArticle(CreateArticleDto createArticleDto)
    {
        var article = new Article
        {
            Id = Guid.NewGuid().ToString(),
            Title = createArticleDto.Title,
            Content = createArticleDto.Content,
            Continent = createArticleDto.Continent,
            Draft = createArticleDto.Draft,
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

    public async Task<List<ArticleResponseDto>> GetLatestArticles()
    {
        var articles = await articleRepository.GetLatestArticles();

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
        var article = await articleRepository.GetArticleById(articleId);
        if (article == null) throw new ArgumentException("No article found with the given ID.");

        // Fetch comments for the article from CommentService
        var response = await _commentClient.GetAsync(
            $"CommentService/getCommentCache/{articleId}");

        response.EnsureSuccessStatusCode();

        var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();

        return new ArticleResponseDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Continent = article.Continent,
            Draft = article.Draft,
            CreatedAt = article.CreatedAt,
            Comments = comments?.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                Message = c.Message,
                Author = c.Author,
                CreatedAt = c.CreatedAt
            }).ToList()
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

    public async Task<ArticleResponseDto> GetRandomArticle()
    {
        var articles = await articleRepository.GetAllArticles();
        if (articles.Count == 0) throw new InvalidOperationException("No articles found.");

        var random = new Random();
        var randomArticle = articles[random.Next(articles.Count)];

        return new ArticleResponseDto
        {
            Id = randomArticle.Id,
            Title = randomArticle.Title,
            Content = randomArticle.Content,
            Continent = randomArticle.Continent,
            Draft = randomArticle.Draft,
            CreatedAt = randomArticle.CreatedAt
        };
    }
}