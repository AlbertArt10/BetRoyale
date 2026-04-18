using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BetRoyale.API.Configurations;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BetRoyale.API.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AuthTokenResult GenerateAccessToken(User user, string roleName)
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer) ||
            string.IsNullOrWhiteSpace(_jwtOptions.Audience) ||
            string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT configuration is incomplete.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, roleName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return new AuthTokenResult
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
