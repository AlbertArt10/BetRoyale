namespace BetRoyale.API.Entities;

public class Article
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }

    public Guid MatchId { get; set; }

    public User Author { get; set; } = null!;

    public Match Match { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<ArticleLike> ArticleLikes { get; set; } = new List<ArticleLike>();
}
