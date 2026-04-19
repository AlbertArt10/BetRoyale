using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        if (!context.Request.Path.Equals("/api/auth/me", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }

                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Authentication is required to access this endpoint."
                        });
                    },
                    OnForbidden = async context =>
                    {
                        var isAdminRequest = context.Request.Path.StartsWithSegments("/api/admin", StringComparison.OrdinalIgnoreCase);
                        if (isAdminRequest)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                message = "Only Admin users can access this endpoint."
                            });

                            return;
                        }

                        var isMatchesWriteRequest =
                            context.Request.Path.StartsWithSegments("/api/matches", StringComparison.OrdinalIgnoreCase) &&
                            (HttpMethods.IsPost(context.Request.Method) ||
                             HttpMethods.IsPut(context.Request.Method) ||
                             HttpMethods.IsDelete(context.Request.Method));

                        if (!isMatchesWriteRequest)
                        {
                            return;
                        }

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Only Admin users can create, update, or delete matches."
                        });
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
