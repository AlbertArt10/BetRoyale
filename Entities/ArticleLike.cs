namespace BetRoyale.API.Entities;

public class ArticleLike
{
    public Guid UserId { get; set; }

    public Guid ArticleId { get; set; }

    public User User { get; set; } = null!;

    public Article Article { get; set; } = null!;
}
