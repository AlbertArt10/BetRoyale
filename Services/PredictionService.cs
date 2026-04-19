using BetRoyale.API.Data;
using BetRoyale.API.DTOs.Predictions;
using BetRoyale.API.Entities;
using BetRoyale.API.Enums;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class PredictionService : IPredictionService
{
    private readonly AppDbContext _dbContext;

    public PredictionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PredictionDetailsDto> CreateAsync(
        CreatePredictionRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var (matchId, outcome) = ValidateCreateRequest(request.MatchId, request.Outcome);

        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(currentUser => currentUser.Role)
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new PredictionUserNotFoundException($"Prediction user '{userId}' was not found.");
        }

        var match = await _dbContext.Matches
            .AsNoTracking()
            .SingleOrDefaultAsync(currentMatch => currentMatch.Id == matchId, cancellationToken);

        if (match is null)
        {
            throw new MatchNotFoundException($"Match '{matchId}' was not found.");
        }

        if (match.Status != MatchStatus.Scheduled)
        {
            throw new InvalidPredictionException("Predictions can only be created for scheduled matches.");
        }

        ValidateOutcomeForSport(match.Sport, outcome);

        var predictionExists = await _dbContext.Predictions
            .AsNoTracking()
            .AnyAsync(
                prediction => prediction.UserId == userId && prediction.MatchId == matchId,
                cancellationToken);

        if (predictionExists)
        {
            throw new InvalidPredictionException("User already has a prediction for this match.");
        }

        var prediction = new Prediction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MatchId = matchId,
            Outcome = outcome
        };

        _dbContext.Predictions.Add(prediction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(prediction, user.Username, match.Sport);
    }

    public async Task<IReadOnlyList<PredictionListItemDto>> GetByMatchIdAsync(
        Guid matchId,
        CancellationToken cancellationToken = default)
    {
        ValidateMatchId(matchId);

        var matchExists = await _dbContext.Matches
            .AsNoTracking()
            .AnyAsync(match => match.Id == matchId, cancellationToken);

        if (!matchExists)
        {
            throw new MatchNotFoundException($"Match '{matchId}' was not found.");
        }

        return await _dbContext.Predictions
            .AsNoTracking()
            .Where(prediction => prediction.MatchId == matchId)
            .OrderBy(prediction => prediction.Id)
            .Select(prediction => new PredictionListItemDto
            {
                Id = prediction.Id,
                UserId = prediction.UserId,
                Username = prediction.User.Username,
                MatchId = prediction.MatchId,
                Sport = prediction.Match.Sport,
                Outcome = prediction.Outcome,
                IsCorrect = prediction.IsCorrect,
                PointsAwarded = prediction.PointsAwarded
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PredictionListItemDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new PredictionUserNotFoundException($"Prediction user '{userId}' was not found.");
        }

        return await _dbContext.Predictions
            .AsNoTracking()
            .Where(prediction => prediction.UserId == userId)
            .OrderBy(prediction => prediction.Match.MatchDate)
            .Select(prediction => new PredictionListItemDto
            {
                Id = prediction.Id,
                UserId = prediction.UserId,
                Username = prediction.User.Username,
                MatchId = prediction.MatchId,
                Sport = prediction.Match.Sport,
                Outcome = prediction.Outcome,
                IsCorrect = prediction.IsCorrect,
                PointsAwarded = prediction.PointsAwarded
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PredictionDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new InvalidPredictionException("PredictionId is required.");
        }

        var prediction = await _dbContext.Predictions
            .AsNoTracking()
            .Where(currentPrediction => currentPrediction.Id == id)
            .Select(currentPrediction => new PredictionDetailsDto
            {
                Id = currentPrediction.Id,
                UserId = currentPrediction.UserId,
                Username = currentPrediction.User.Username,
                MatchId = currentPrediction.MatchId,
                Sport = currentPrediction.Match.Sport,
                Outcome = currentPrediction.Outcome,
                IsCorrect = currentPrediction.IsCorrect,
                PointsAwarded = currentPrediction.PointsAwarded
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (prediction is null)
        {
            throw new PredictionNotFoundException($"Prediction '{id}' was not found.");
        }

        return prediction;
    }

    private static (Guid MatchId, PredictionOutcome Outcome) ValidateCreateRequest(Guid matchId, PredictionOutcome outcome)
    {
        ValidateMatchId(matchId);

        if (!Enum.IsDefined(outcome))
        {
            throw new InvalidPredictionException("Prediction outcome value is invalid.");
        }

        return (matchId, outcome);
    }

    private static void ValidateOutcomeForSport(SportType sport, PredictionOutcome outcome)
    {
        if ((sport == SportType.Tennis || sport == SportType.Basketball) && outcome == PredictionOutcome.Draw)
        {
            throw new InvalidPredictionException("Draw predictions are allowed only for football matches.");
        }
    }

    private static void ValidateMatchId(Guid matchId)
    {
        if (matchId == Guid.Empty)
        {
            throw new InvalidPredictionException("MatchId is required.");
        }
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidPredictionException("UserId is required.");
        }
    }

    private static PredictionDetailsDto MapToDetailsDto(Prediction prediction, string username, SportType sport)
    {
        return new PredictionDetailsDto
        {
            Id = prediction.Id,
            UserId = prediction.UserId,
            Username = username,
            MatchId = prediction.MatchId,
            Sport = sport,
            Outcome = prediction.Outcome,
            IsCorrect = prediction.IsCorrect,
            PointsAwarded = prediction.PointsAwarded
        };
    }
}
