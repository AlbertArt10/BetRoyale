namespace BetRoyale.API.DTOs.Auth;

public class AuthResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? AccessToken { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }
}
