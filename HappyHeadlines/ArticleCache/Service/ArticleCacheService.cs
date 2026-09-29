using System.Text.Json;
using CoreDomain;
using StackExchange.Redis;

namespace ArticleCache.Service;

public class ArticleCacheService(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory) : IArticleCacheService
{
    private readonly HttpClient articleClient = httpClientFactory.CreateClient("ArticleService");

    private readonly IDatabase database = redis.GetDatabase();

    //TODO: Change when this should be called. (Maybe new endpoint in article service, that returns the 10 most recent articles and then it should call articleCacheService from there)
        
    public async Task<Article> GetArticleAsync(string articleId)
    {
        var key = $"article-{articleId}";
        
        var value = await database.StringGetAsync(key);
        
        if (value == RedisValue.Null)
        {
            var response = await articleClient.GetAsync($"ArticleService/getArticleById/{articleId}");
            
            response.EnsureSuccessStatusCode();

            var article = response.Content.ReadFromJsonAsync<Article>();
            
            return article.Result ?? throw new Exception("Failed to retrieve article from ArticleService.");
        }

        return JsonSerializer.Deserialize<Article>(value.ToString())!;
    }

    public async Task<List<Article>> GetArticlesAsync()
    {
        //Todo: Implement this method to retrieve all articles from the cache or ArticleService if not present in cache.
        throw new NotImplementedException();
    }

    public async Task GetLatestArticlesAsync()
    {
        var response = await articleClient.GetAsync("ArticleService/getLatestArticles");

        response.EnsureSuccessStatusCode();

        var articles = await response.Content.ReadFromJsonAsync<List<Article>>();

        if (articles == null)
        {
            throw new Exception("Failed to retrieve articles from ArticleService.");
        }

        var server = redis.GetServer(redis.GetEndPoints().First());

        var articleKeys = server.Keys(pattern: "article-*").ToArray();

        if (articleKeys.Length > 0)
        {
            await database.KeyDeleteAsync(articleKeys);
        }

        foreach (var article in articles)
        {
            var key = $"article-{article.Id}";
            var value = JsonSerializer.Serialize(article);

            await database.StringSetAsync(key, value);
        }
        Console.WriteLine($"Cached {articles.Count} articles in Redis.");
    }
}