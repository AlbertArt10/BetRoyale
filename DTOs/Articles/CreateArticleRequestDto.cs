namespace BetRoyale.API.DTOs.Articles;

public class CreateArticleRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid MatchId { get; set; }
}
