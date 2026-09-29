using ArticleDatabase;
using ArticleService.Interface;
using CoreDomain;

namespace ArticleService;

public class ArticleService(IHttpClientFactory httpClientFactory) : IArticleService
{
    private readonly HttpClient commentClient = httpClientFactory.CreateClient("CommentService");
    
    private readonly Database database = Database.GetInstance();

    public async Task CreateArticle(Article article)
    {
        database.InsertArticle(article);
    }

    public Task<List<Article>> GetLatestArticles()
    {
        var articles = database.GetLatestArticles();
        return Task.FromResult(articles);
    }
    
    public async Task<Article> GetArticleById(string articleId)
    {
        var article = database.GetArticleById(articleId);
        if (article == null)
        {
            throw new ArgumentException("No article found with the given ID.");
        }

        // Fetch comments for the article from CommentService
        var response = await commentClient.GetAsync(
            $"CommentService/getCommentCache/{articleId}");        
        
        response.EnsureSuccessStatusCode();
        
        var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();
        
        article.Comments = comments ?? new List<Comment>();
        
        return article;
    }

    public Task<List<Article>> GetArticles()
    {
        var articles = database.GetAllArticles();
        return Task.FromResult(articles);
    }

    public Task UpdateArticle(string id, Article article)
    {
       database.UpdateArticle(id, article);
       return Task.CompletedTask;
    }

    public Task DeleteArticle(string articleId)
    {
        database.DeleteArticle(articleId);
        return Task.CompletedTask;
    }

    public Task<Article> GetRandomArticle()
    {
        var articles = database.GetAllArticles();
        if (articles.Count == 0)
        {
            throw new InvalidOperationException("No articles found.");
        }

        var random = new Random();
        var newestArticle = articles[random.Next(articles.Count)];
        
        return Task.FromResult(newestArticle);
    }
}