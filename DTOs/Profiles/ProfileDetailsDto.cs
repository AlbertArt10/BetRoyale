namespace BetRoyale.API.DTOs.Profiles;

public class ProfileDetailsDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public int TotalPoints { get; set; }
}
