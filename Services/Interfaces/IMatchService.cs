using BetRoyale.API.DTOs.Matches;

namespace BetRoyale.API.Services.Interfaces;

public interface IMatchService
{
    Task<MatchDetailsDto> CreateAsync(CreateMatchRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MatchListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MatchDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MatchDetailsDto> UpdateAsync(Guid id, UpdateMatchRequestDto request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
