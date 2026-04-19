namespace BetRoyale.API.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid ArticleId { get; set; }

    public User User { get; set; } = null!;

    public Article Article { get; set; } = null!;
}
