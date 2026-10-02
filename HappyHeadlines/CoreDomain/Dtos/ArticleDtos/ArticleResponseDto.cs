using CoreDomain.Dtos.CommentDtos;

namespace CoreDomain.Dtos.ArticleDtos;

public class ArticleResponseDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool Draft { get; set; }

    public List<CommentResponseDto>? Comments { get; set; }
}