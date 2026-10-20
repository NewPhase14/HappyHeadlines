using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ArticleDatabase.Repository;

public class ArticleRepository : IArticleRepository
{
    private readonly Coordinator _coordinator = new();

    // Get all articles from all databases
    public async Task<List<Article>> GetAllArticles()
    {
        var articles = new List<Article>();

        foreach (var context in _coordinator.GetAllContexts())
            await using (context)
            {
                articles.AddRange(
                    await context.Articles.ToListAsync()
                );
            }

        return articles;
    }

    // Get an article by its ID from the appropriate database
    public async Task<Article?> GetArticleById(string id)
    {
        foreach (var context in _coordinator.GetAllContexts())
            await using (context)
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article != null)
                    return article;
            }

        return null;
    }

    // Get the latest articles from the last 14 days
    public async Task<List<Article>> GetArticlesFromLast14Days()
    {
        var articles = new List<Article>();

        var cutoff = DateTime.UtcNow.AddDays(-14);

        foreach (var context in _coordinator.GetAllContexts())
            await using (context)
            {
                var results = await context.Articles
                    .Where(a => a.CreatedAt >= cutoff)
                    .ToListAsync();

                articles.AddRange(results);
            }

        return articles;
    }

    // Create a new article in the appropriate database based on its continent
    public async Task<Article> CreateArticle(Article article)
    {
        await using var context =
            _coordinator.GetContext(article.Continent);

        context.Articles.Add(article);
        await context.SaveChangesAsync();

        return article;
    }

    // Update an existing article by its ID
    public async Task<Article?> UpdateArticle(Article article)
    {
        foreach (var context in _coordinator.GetAllContexts())
            await using (context)
            {
                var existingArticle = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == article.Id);

                if (existingArticle == null)
                    continue;

                existingArticle.Title = article.Title;
                existingArticle.Content = article.Content;
                existingArticle.Continent = article.Continent;
                existingArticle.Draft = article.Draft;

                await context.SaveChangesAsync();

                return existingArticle;
            }

        return null;
    }

    // Delete an article by its ID
    public async Task DeleteArticle(string id)
    {
        foreach (var context in _coordinator.GetAllContexts())
            await using (context)
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                    continue;

                context.Articles.Remove(article);
                await context.SaveChangesAsync();
            }
    }
}