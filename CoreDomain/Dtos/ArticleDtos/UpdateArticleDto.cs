namespace CoreDomain.Dtos.ArticleDtos;

public class UpdateArticleDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public bool Draft { get; set; }
}