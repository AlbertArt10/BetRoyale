using BetRoyale.API.DTOs.Profiles;

namespace BetRoyale.API.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDetailsDto> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ProfileDetailsDto> UpdateMyProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default);
}
