using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BetRoyale.API.Configurations;

public static class AuthenticationServiceCollectionExtensions
{
    private const string DevelopmentFallbackIssuer = "BetRoyale.API";
    private const string DevelopmentFallbackAudience = "BetRoyale.Client";
    private const string DevelopmentFallbackSecretKey = "development-placeholder-secret-key-change-later";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var issuer = string.IsNullOrWhiteSpace(jwtOptions.Issuer) ? DevelopmentFallbackIssuer : jwtOptions.Issuer;
        var audience = string.IsNullOrWhiteSpace(jwtOptions.Audience) ? DevelopmentFallbackAudience : jwtOptions.Audience;
        var secretKey = string.IsNullOrWhiteSpace(jwtOptions.SecretKey) ? DevelopmentFallbackSecretKey : jwtOptions.SecretKey;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        return services;
    }
}
