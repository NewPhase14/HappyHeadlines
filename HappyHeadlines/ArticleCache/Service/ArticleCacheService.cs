using System.Text.Json;
using CoreDomain;
using StackExchange.Redis;

namespace ArticleCache.Service;

public class ArticleCacheService(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory)
    : IArticleCacheService
{
    private readonly HttpClient articleClient = httpClientFactory.CreateClient("ArticleService");

    private readonly IDatabase database = redis.GetDatabase();

    // This method retrieves an article from the cache.
    public async Task<Article?> GetArticleById(string articleId)
    {
        var key = $"article-{articleId}";

        var value = await database.StringGetAsync(key);
        
        if (value == RedisValue.Null)
        {
            Console.WriteLine("Redis value == null");
            return null;
        }

        return JsonSerializer.Deserialize<Article>(value.ToString())!;
    }
    
    // This method refreshes the cache by fetching the articles from the latest 14 days.
    public async Task RefreshCache()
    {
        var response = await articleClient.GetAsync("ArticleService/getArticlesFromLast14Days");

        response.EnsureSuccessStatusCode();

        var articles = await response.Content.ReadFromJsonAsync<List<Article>>();

        if (articles == null) throw new Exception("Failed to retrieve articles from ArticleService.");

        var server = redis.GetServer(redis.GetEndPoints().First());

        var articleKeys = server.Keys(pattern: "article-*").ToArray();

        if (articleKeys.Length > 0) await database.KeyDeleteAsync(articleKeys);

        foreach (var article in articles)
        {
            var key = $"article-{article.Id}";
            var value = JsonSerializer.Serialize(article);

            await database.StringSetAsync(key, value);
        }

        Console.WriteLine($"Cached {articles.Count} articles in Redis.");
    }
}