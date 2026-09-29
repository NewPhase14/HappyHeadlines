using CommentDatabase;
using CoreDomain;

namespace CommentService.Service;

public class CommentService(IHttpClientFactory httpClientFactory) : ICommentService
{
    private readonly Database database = Database.GetInstance();
    private readonly HttpClient profanityClient = httpClientFactory.CreateClient("ProfanityService");
    private readonly HttpClient commentCacheClient = httpClientFactory.CreateClient("CommentCacheService");
    
    public async Task CreateComment(Comment comment)
    {
        try
        {
            var response = await profanityClient.PostAsJsonAsync(
                "ProfanityService/checkProfanity",
                comment.Message);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Profanity service is unavailable.");
            }

            var containsProfanity =
                await response.Content.ReadFromJsonAsync<bool>();

            if (containsProfanity)
            {
                throw new HttpRequestException(
                    "Comment contains profanity and cannot be added.");
            }
        }
        catch (HttpRequestException)
        {
            throw new HttpRequestException(
                "Profanity service is unavailable.");
        }
        try
        {
            database.InsertComment(comment);
        }
        catch
        {
            throw new HttpRequestException(
                "Comment database is unavailable.");
        }
    }

    public Task<List<Comment>> GetComments()
    {
        var comments = database.GetAllComments();
        
        return Task.FromResult(comments);
    }

    public async Task<List<Comment>> GetCommentCacheAsync(string articleId)
    {
        var response = await commentCacheClient.GetAsync($"CommentCache/getCommentsByArticleId/{articleId}");

        var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();
        
        return comments ?? new List<Comment>();
    }

    public Task<List<Comment>> GetCommentsByArticleId(string articleId)
    {
        var comments = database.GetCommentsByArticleId(articleId);
        return Task.FromResult(comments);
    }
}