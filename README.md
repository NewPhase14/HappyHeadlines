## GAF Tactics 

1. [Avoid Unncessary Resources] - Stop fetching all articles, make the database return a single article.

This tactic includes the following: 
- Defer Fetching/Lazy Fetch
- Reduce Network Package Size

### Before GAF implementation
- Before the implementation, we fetched all articles and then selected a random article from that list. 
```csharp
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
```

### After GAF implementation
- Now we added a new method in repository, so that we only fetch the lastest article from the database. We decided to minimize the daily article to only include articles from the global database.
- Of course this has a tradeoff, as the daily article isn't that random anymore, but we decided that this tradeoff was worth it, as we are now saving a lot of resources by not fetching all articles from the database.
```csharp
public async Task<Article?> GetLatestArticle()
    {
        await using var context = _coordinator.GetContext("Global");

        var articles = await context.Articles.OrderBy(a => a.CreatedAt).LastOrDefaultAsync();

        return articles ?? null;
    }
```

2. [Insert GAF tactic here] - [Insert GAF tactic description here]

3. [Insert GAF tactic here] - [Insert GAF tactic description here]

