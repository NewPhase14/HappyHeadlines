namespace CoreDomain.Dtos.ArticleDtos;

public class CreateArticleDto
{
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Continent { get; set; } = null!;
}