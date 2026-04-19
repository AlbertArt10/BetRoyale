using BetRoyale.API.Data;
using BetRoyale.API.Data.Seed;
using BetRoyale.API.DTOs.Admin;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashingService _passwordHashingService;

    public AdminService(AppDbContext dbContext, IPasswordHashingService passwordHashingService)
    {
        _dbContext = dbContext;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<IReadOnlyList<UserRoleResponseDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .OrderBy(user => user.Username)
            .Select(user => new UserRoleResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminUserDetailsDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidUserRoleChangeException("UserId is required.");
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(currentUser => currentUser.Role)
            .Include(currentUser => currentUser.UserProfile)
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException($"User '{userId}' was not found.");
        }

        return MapToAdminUserDetailsDto(user);
    }

    public async Task<AdminUserDetailsDto> CreateUserAsync(
        CreateAdminUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var username = ValidateUsername(request.Username);
        var email = ValidateEmail(request.Email);
        var password = ValidatePassword(request.Password);
        var role = await ResolveTargetRoleAsync(request.RoleName, cancellationToken);

        await EnsureUsernameAndEmailAreUniqueAsync(username, email, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            RoleId = role.Id
        };

        user.PasswordHash = _passwordHashingService.HashPassword(user, password);

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

        user.Role = role;
        user.UserProfile = userProfile;

        return MapToAdminUserDetailsDto(user);
    }

    public async Task<AdminUserDetailsDto> UpdateUserAsync(
        Guid userId,
        UpdateAdminUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidUserRoleChangeException("UserId is required.");
        }

        var user = await _dbContext.Users
            .Include(currentUser => currentUser.Role)
            .Include(currentUser => currentUser.UserProfile)
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException($"User '{userId}' was not found.");
        }

        if (user.RoleId == RoleSeedData.AdminId)
        {
            throw new InvalidUserRoleChangeException("Admin users cannot be changed with this endpoint.");
        }

        var username = ValidateUsername(request.Username);
        var email = ValidateEmail(request.Email);
        var role = await ResolveTargetRoleAsync(request.RoleName, cancellationToken);

        await EnsureUsernameAndEmailAreUniqueForUpdateAsync(userId, username, email, cancellationToken);

        user.Username = username;
        user.Email = email;
        user.RoleId = role.Id;

        await _dbContext.SaveChangesAsync(cancellationToken);

        user.Role = role;

        return MapToAdminUserDetailsDto(user);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidUserRoleChangeException("UserId is required.");
        }

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException($"User '{userId}' was not found.");
        }

        if (user.RoleId == RoleSeedData.AdminId)
        {
            throw new InvalidUserRoleChangeException("Admin users cannot be deleted with this endpoint.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUsernameAndEmailAreUniqueAsync(string username, string email, CancellationToken cancellationToken)
    {
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
    }

    private async Task EnsureUsernameAndEmailAreUniqueForUpdateAsync(
        Guid userId,
        string username,
        string email,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToUpperInvariant();

        var usernameExists = await _dbContext.Users
            .AnyAsync(user => user.Id != userId && user.Username == username, cancellationToken);
        if (usernameExists)
        {
            throw new DuplicateUsernameException($"Username '{username}' is already taken.");
        }

        var emailExists = await _dbContext.Users
            .AnyAsync(user => user.Id != userId && user.Email.ToUpper() == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            throw new DuplicateEmailException($"Email '{email}' is already registered.");
        }
    }

    private async Task<Role> ResolveTargetRoleAsync(string? roleName, CancellationToken cancellationToken)
    {
        var normalizedRoleName = roleName?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedRoleName))
        {
            throw new InvalidUserRoleChangeException("RoleName is required.");
        }

        var targetRole = normalizedRoleName.ToUpperInvariant() switch
        {
            "USER" => RoleSeedData.User,
            "ANALYST" => RoleSeedData.Analyst,
            _ => throw new InvalidUserRoleChangeException("RoleName must be either 'User' or 'Analyst'.")
        };

        var roleExists = await _dbContext.Roles
            .AsNoTracking()
            .AnyAsync(role => role.Id == targetRole.Id, cancellationToken);

        if (!roleExists)
        {
            throw new RoleNotFoundException($"Role '{targetRole.Name}' was not found.");
        }

        return targetRole;
    }

    private static string ValidateUsername(string? username)
    {
        var normalizedUsername = username?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedUsername))
        {
            throw new InvalidUserRoleChangeException("Username is required.");
        }

        return normalizedUsername;
    }

    private static string ValidateEmail(string? email)
    {
        var normalizedEmail = email?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new InvalidUserRoleChangeException("Email is required.");
        }

        return normalizedEmail;
    }

    private static string ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            throw new InvalidUserRoleChangeException("Password must be at least 8 characters long.");
        }

        return password;
    }

    private static AdminUserDetailsDto MapToAdminUserDetailsDto(User user)
    {
        return new AdminUserDetailsDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.Name,
            FullName = user.UserProfile?.FullName,
            Bio = user.UserProfile?.Bio,
            TotalPoints = user.UserProfile?.TotalPoints ?? 0
        };
    }
}
