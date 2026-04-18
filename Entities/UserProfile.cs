namespace BetRoyale.API.Entities;

public class UserProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public int TotalPoints { get; set; }

    public User User { get; set; } = null!;
}
