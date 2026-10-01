using System.Text.Json;
using CoreDomain;
using StackExchange.Redis;

namespace CommentCache.Service;

public class CommentCacheService(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory)
    : ICommentCacheService
{
    private readonly HttpClient commentClient = httpClientFactory.CreateClient("CommentService");

    private readonly IDatabase database = redis.GetDatabase();

    public async Task<List<Comment>> GetCommentsByArticleId(string articleId)
    {
        // Check if comments for the article are already cached
        var key = $"comments-{articleId}";
        var cachedComments = await database.StringGetAsync(key);

        if (cachedComments.HasValue)
        {
            // TODO: Always returns what got cached first time the cached was missed. Needs to update to get new comments. 

            var cachedCommentsList = JsonSerializer.Deserialize<List<Comment>>(
                cachedComments.ToString());

            if (cachedCommentsList != null && cachedCommentsList.Count > 0)
            {
                Console.WriteLine("Cache hit");
                return cachedCommentsList;
            }

            Console.WriteLine("Cache contains [], treating as cache miss");
        }

        Console.WriteLine("Cache miss");

        // If not, fetch comments from CommentService and cache them
        var response = await commentClient.GetAsync($"CommentService/getCommentsByArticleId/{articleId}");

        response.EnsureSuccessStatusCode();

        var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();

        if (comments == null) throw new Exception("Failed to retrieve comments from CommentService.");

        //TODO: Ensure cache is limited to contain all comments for the most recently 30 accessed articles

        // Cache new entry 
        var value = JsonSerializer.Serialize(comments);
        await database.StringSetAsync(key, value);

        Console.WriteLine($"value: {value}");

        Console.WriteLine($"Cached: {key} -> {value}");

        return comments;
    }
}