using BetRoyale.API.DTOs.Admin;

namespace BetRoyale.API.Services.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyList<UserRoleResponseDto>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<AdminUserDetailsDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AdminUserDetailsDto> CreateUserAsync(
        CreateAdminUserRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AdminUserDetailsDto> UpdateUserAsync(
        Guid userId,
        UpdateAdminUserRequestDto request,
        CancellationToken cancellationToken = default);

    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
