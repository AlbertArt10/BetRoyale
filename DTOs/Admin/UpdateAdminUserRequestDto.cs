namespace BetRoyale.API.DTOs.Admin;

public class UpdateAdminUserRequestDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
