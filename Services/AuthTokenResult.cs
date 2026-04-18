namespace BetRoyale.API.Services;

public class AuthTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
}
