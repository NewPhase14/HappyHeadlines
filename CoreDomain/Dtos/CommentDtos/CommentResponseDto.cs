namespace CoreDomain.Dtos.CommentDtos;

public class CommentResponseDto
{
    public string Id { get; set; }
    public string ArticleId { get; set; }
    public string Message { get; set; }
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
}