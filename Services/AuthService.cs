using BetRoyale.API.Configurations;
using BetRoyale.API.Data;
using BetRoyale.API.Data.Seed;
using BetRoyale.API.DTOs.Auth;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BetRoyale.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHashingService passwordHashingService,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHashingService = passwordHashingService;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        _ = _jwtOptions;

        var username = request.Username?.Trim();
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidUsernameException("Username is required.");
        }

        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException("Email is required.");
        }

        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
        {
            throw new InvalidPasswordException("Password must be at least 8 characters long.");
        }

        var normalizedEmail = email.ToUpperInvariant();

        var usernameExists = await _dbContext.Users
            .AnyAsync(user => user.Username == username, cancellationToken);
        if (usernameExists)
        {
            throw new DuplicateUsernameException($"Username '{username}' is already taken.");
        }

        var emailExists = await _dbContext.Users
            .AnyAsync(user => user.Email.ToUpper() == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            throw new DuplicateEmailException($"Email '{email}' is already registered.");
        }

        var defaultRole = await _dbContext.Roles
            .SingleOrDefaultAsync(role => role.Id == RoleSeedData.UserId, cancellationToken);
        if (defaultRole is null)
        {
            throw new RoleNotFoundException("Default role 'User' was not found.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            RoleId = defaultRole.Id
        };

        user.PasswordHash = _passwordHashingService.HashPassword(user, request.Password);

        var userProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = null,
            Bio = null,
            TotalPoints = 0
        };

        _dbContext.Users.Add(user);
        _dbContext.UserProfiles.Add(userProfile);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = defaultRole.Name,
            AccessToken = null,
            ExpiresAtUtc = null
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        _ = _jwtOptions;

        var emailOrUsername = request.EmailOrUsername?.Trim();
        if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrEmpty(request.Password))
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        var normalizedIdentifier = emailOrUsername.ToUpperInvariant();

        var user = await _dbContext.Users
            .Include(currentUser => currentUser.Role)
            .SingleOrDefaultAsync(
                currentUser =>
                    currentUser.Username == emailOrUsername ||
                    currentUser.Email.ToUpper() == normalizedIdentifier,
                cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        var isPasswordValid = _passwordHashingService.VerifyPassword(user, user.PasswordHash, request.Password);
        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        if (user.Role is null)
        {
            throw new RoleNotFoundException("The user's role was not found.");
        }

        var tokenResult = _tokenService.GenerateAccessToken(user, user.Role.Name);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.Name,
            AccessToken = tokenResult.AccessToken,
            ExpiresAtUtc = tokenResult.ExpiresAtUtc
        };
    }
}
