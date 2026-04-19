namespace BetRoyale.API.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public UserProfile? UserProfile { get; set; }

    public ICollection<Article> Articles { get; set; } = new List<Article>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<ArticleLike> ArticleLikes { get; set; } = new List<ArticleLike>();

    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
}
