namespace BetRoyale.API.DTOs.Auth;

public class LoginRequestDto
{
    public string EmailOrUsername { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
