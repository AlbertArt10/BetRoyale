using BetRoyale.API.Data;
using BetRoyale.API.DTOs.Profiles;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _dbContext;

    public ProfileService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileDetailsDto> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(currentUser => currentUser.Role)
            .Include(currentUser => currentUser.UserProfile)
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null || user.UserProfile is null)
        {
            throw new ProfileNotFoundException($"Profile for user '{userId}' was not found.");
        }

        return MapToDetailsDto(user);
    }

    public async Task<ProfileDetailsDto> UpdateMyProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var user = await _dbContext.Users
            .Include(currentUser => currentUser.Role)
            .Include(currentUser => currentUser.UserProfile)
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null || user.UserProfile is null)
        {
            throw new ProfileNotFoundException($"Profile for user '{userId}' was not found.");
        }

        user.UserProfile.FullName = ValidateFullName(request.FullName);
        user.UserProfile.Bio = ValidateBio(request.Bio);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(user);
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidProfileException("UserId is required.");
        }
    }

    private static string? ValidateFullName(string? fullName)
    {
        var normalizedFullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim();

        if (normalizedFullName is not null && normalizedFullName.Length > 150)
        {
            throw new InvalidProfileException("FullName must be 150 characters or fewer.");
        }

        return normalizedFullName;
    }

    private static string? ValidateBio(string? bio)
    {
        var normalizedBio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();

        if (normalizedBio is not null && normalizedBio.Length > 1000)
        {
            throw new InvalidProfileException("Bio must be 1000 characters or fewer.");
        }

        return normalizedBio;
    }

    private static ProfileDetailsDto MapToDetailsDto(Entities.User user)
    {
        return new ProfileDetailsDto
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
