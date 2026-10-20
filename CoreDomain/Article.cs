namespace CoreDomain;

public class Article
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool Draft { get; set; }
}