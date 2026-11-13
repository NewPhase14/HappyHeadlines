# GAF Tactics 

## 1. [Avoid Unncessary Resources] - Stop fetching all articles, make the database return a single article.

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

## 2. [Utilize an Efficient Technology] - Add index to the CreatedAt column in the database, so that we can efficiently fetch the latest article.
### Before GAF implementation
- Before the implementation, we didn't have an index on the CreatedAt column, so fetching the latest article was inefficient, as the database had to scan all articles to find the latest one.
```csharp
using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ArticleDatabase;

public class ArticleContext(DbContextOptions<ArticleContext> options) : DbContext(options)
{
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            entity.ToTable("article");

            entity.Property(a => a.Id)
                .HasColumnName("id");

            entity.Property(a => a.Title)
                .HasColumnName("title");

            entity.Property(a => a.Content)
                .HasColumnName("content");

            entity.Property(a => a.Continent)
                .HasColumnName("continent")
                .HasMaxLength(50);

            entity.Property(a => a.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(a => a.Draft)
                .HasColumnName("draft");
        });
    }
}
```

### After GAF implementation
- Now we added an index on the CreatedAt column, so fetching the latest article is efficient, as the database can use the index to find the latest article.
```csharp
using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ArticleDatabase;

public class ArticleContext(DbContextOptions<ArticleContext> options) : DbContext(options)
{
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            entity.HasIndex(a => a.CreatedAt);

            entity.ToTable("article");

            entity.Property(a => a.Id)
                .HasColumnName("id");

            entity.Property(a => a.Title)
                .HasColumnName("title");

            entity.Property(a => a.Content)
                .HasColumnName("content");

            entity.Property(a => a.Continent)
                .HasColumnName("continent")
                .HasMaxLength(50);

            entity.Property(a => a.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(a => a.Draft)
                .HasColumnName("draft");
        });
    }
}
```

## 3. [Fetch Data from the Proximity] - Add in-memory caching to the profanity service, so that we don't have to fetch all the profanities from the database every time we want to check for profanity in a message.

This tactic includes the following:
- Cache data closer to the user

### Before GAF implementation
- Before the implementation, we fetched all profanities from the database every time we wanted to check for profanity in a message.
```csharp
public class ProfanityRepository(ProfanityContext context) : IProfanityRepository
{
    public async Task<Profanity> CreateProfanity(Profanity profanity)
    {
        context.Profanity.Add(profanity);
        await context.SaveChangesAsync();
        return profanity;
    }

    public async Task<bool> ContainsProfanity(string message)
    {
        var words = await context.Profanity.Select(p => p.Word).ToListAsync();

        return words.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase));
    }
```

### After GAF implementation
- Now we added in-memory caching to the profanity service, so that we only fetch all profanity words once every hour, and then we cache the words in memory, so that we don't have to fetch them from the database every time we want to check for profanity in a message.
- We also added a cache invalidation mechanism, so that when we add a new profanity word to the database, we invalidate the cache, so that the next time we check for profanity in a message, we fetch the updated list of profanity words from the database.
```csharp
public class ProfanityRepository(ProfanityContext context, IMemoryCache memoryCache) : IProfanityRepository
{
    public async Task<Profanity> CreateProfanity(Profanity profanity)
    {
        context.Profanity.Add(profanity);
        await context.SaveChangesAsync();
        memoryCache.Remove("profanity-words");
        return profanity;
    }

    public async Task<bool> ContainsProfanity(string message)
    {
        var words = await memoryCache.GetOrCreateAsync("profanity-words", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await context.Profanity.Select(p => p.Word).ToListAsync();
        });
        if (words == null) return false;

        return words.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase));
    }
```
