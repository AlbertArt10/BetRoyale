namespace BetRoyale.API.DTOs.Admin;

public class AdminPingResponseDto
{
    public string Message { get; set; } = string.Empty;

    public Guid? UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
