using System.Text.Json;
using CoreDomain;
using StackExchange.Redis;

namespace CommentCache.Service;

public class CommentCacheService(
    IConnectionMultiplexer redis,
    IHttpClientFactory httpClientFactory)
    : ICommentCacheService
{
    private const string LruKey = "comment-cache-lru";
    private const int MaxCachedArticles = 30;

    private readonly HttpClient commentClient =
        httpClientFactory.CreateClient("CommentService");

    private readonly IDatabase database = redis.GetDatabase();

    public async Task<List<Comment>> GetCommentsByArticleId(string articleId)
    {
        var key = $"comments-{articleId}";

        // Try cache first
        var cachedComments = await database.StringGetAsync(key);

        if (cachedComments.HasValue)
        {
            var cachedCommentsList =
                JsonSerializer.Deserialize<List<Comment>>(
                    cachedComments.ToString());

            if (cachedCommentsList != null)
            {
                Console.WriteLine("Cache hit");

                // Mark as recently used
                await MarkAsRecentlyUsed(articleId);

                return cachedCommentsList;
            }
        }

        // Cache miss
        Console.WriteLine("Cache miss");

        var response = await commentClient.GetAsync(
            $"CommentService/getCommentsByArticleId/{articleId}");

        response.EnsureSuccessStatusCode();

        var comments =
            await response.Content.ReadFromJsonAsync<List<Comment>>();

        if (comments == null)
        {
            throw new Exception(
                "Failed to retrieve comments from CommentService.");
        }

        // Cache comments
        var value = JsonSerializer.Serialize(comments);

        await database.StringSetAsync(key, value);

        // Mark as recently used
        await MarkAsRecentlyUsed(articleId);

        // Remove old entries if necessary
        await RemoveLeastRecentlyUsed();

        return comments;
    }

    private async Task MarkAsRecentlyUsed(string articleId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await database.SortedSetAddAsync(
            LruKey,
            articleId,
            timestamp);
    }

    private async Task RemoveLeastRecentlyUsed()
    {
        var count = await database.SortedSetLengthAsync(LruKey);

        while (count > MaxCachedArticles)
        {
            var oldest = await database.SortedSetRangeByRankAsync(
                LruKey,
                0,
                0);

            if (oldest.Length == 0)
                return;

            var articleId = oldest[0].ToString();

            await database.KeyDeleteAsync(
                $"comments-{articleId}");

            await database.SortedSetRemoveAsync(
                LruKey,
                articleId);

            count--;
        }
    }
}