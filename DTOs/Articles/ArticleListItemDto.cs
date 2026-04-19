namespace BetRoyale.API.DTOs.Articles;

public class ArticleListItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }

    public string AuthorUsername { get; set; } = string.Empty;

    public Guid MatchId { get; set; }
}
