using BetRoyale.API.DTOs.Predictions;

namespace BetRoyale.API.Services.Interfaces;

public interface IPredictionService
{
    Task<PredictionDetailsDto> CreateAsync(
        CreatePredictionRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PredictionListItemDto>> GetByMatchIdAsync(
        Guid matchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PredictionListItemDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PredictionDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
