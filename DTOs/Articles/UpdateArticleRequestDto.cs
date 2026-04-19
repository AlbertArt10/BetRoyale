namespace BetRoyale.API.DTOs.Articles;

public class UpdateArticleRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid MatchId { get; set; }
}
