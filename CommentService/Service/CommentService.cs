using CommentDatabase.Repository;
using CoreDomain;
using CoreDomain.Dtos.CommentDtos;

namespace CommentService.Service;

public class CommentService(IHttpClientFactory httpClientFactory, ICommentRepository commentRepository)
    : ICommentService
{
    private readonly HttpClient _commentCacheClient = httpClientFactory.CreateClient("CommentCacheService");
    private readonly HttpClient _profanityClient = httpClientFactory.CreateClient("ProfanityService");

    public async Task<CommentResponseDto> CreateComment(CreateCommentDto createCommentDto)
    {
        var response = await _profanityClient.PostAsJsonAsync(
            "ProfanityService/checkProfanity",
            createCommentDto.Message);

        if (!response.IsSuccessStatusCode) throw new HttpRequestException("Profanity service is unavailable.");

        var containsProfanity = await response.Content.ReadFromJsonAsync<bool>();

        if (containsProfanity) throw new Exception("Comment contains profanity and cannot be added.");

        var comment = new Comment
        {
            Id = Guid.NewGuid().ToString(),
            ArticleId = createCommentDto.ArticleId,
            Message = createCommentDto.Message,
            Author = createCommentDto.Author,
            CreatedAt = DateTime.UtcNow
        };

        var createdComment = await commentRepository.CreateComment(comment);
        return new CommentResponseDto
        {
            Id = createdComment.Id,
            ArticleId = createdComment.ArticleId,
            Message = createdComment.Message,
            Author = createdComment.Author,
            CreatedAt = createdComment.CreatedAt
        };
    }

    public async Task<List<CommentResponseDto>> GetAllComments()
    {
        var comments = await commentRepository.GetAllComments();

        if (comments.Count == 0) throw new InvalidOperationException("No comments found.");

        return comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            ArticleId = c.ArticleId,
            Message = c.Message,
            Author = c.Author,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<List<CommentResponseDto>> GetCommentCacheAsync(string articleId)
    {
        var response = await _commentCacheClient.GetAsync($"CommentCache/getCommentsByArticleId/{articleId}");

        var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();

        if (comments == null) throw new InvalidOperationException("No comments found for the specified article.");

        return comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            ArticleId = c.ArticleId,
            Message = c.Message,
            Author = c.Author,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<List<CommentResponseDto>> GetCommentsByArticleId(string articleId)
    {
        var comments = await commentRepository.GetCommentsByArticleId(articleId);
        return (comments ?? throw new InvalidOperationException("No comments found for the specified article."))
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                Message = c.Message,
                Author = c.Author,
                CreatedAt = c.CreatedAt
            }).ToList();
    }
}