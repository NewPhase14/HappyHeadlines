using ArticleDatabase;
using ArticleService.Interface;
using CoreDomain;

namespace ArticleService;

public class ArticleService : IArticleService
{
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
    
    public Task<Article> GetArticleById(string articleId)
    {
        var article = database.GetArticleById(articleId);
        if (article == null)
        {
            throw new ArgumentException("No article found with the given ID.");
        }
        return Task.FromResult(article);
    }
}