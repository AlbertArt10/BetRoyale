namespace BetRoyale.API.DTOs.Articles;

public class ArticleDetailsDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }

    public string AuthorUsername { get; set; } = string.Empty;

    public Guid MatchId { get; set; }
}
