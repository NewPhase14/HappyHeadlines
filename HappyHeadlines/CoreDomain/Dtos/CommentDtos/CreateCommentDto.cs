namespace CoreDomain.Dtos.CommentDtos;

public class CreateCommentDto
{
    public string ArticleId { get; set; }
    public string Message { get; set; }
    public string Author { get; set; }
}