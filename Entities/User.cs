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
}
