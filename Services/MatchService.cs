using BetRoyale.API.Data;
using BetRoyale.API.DTOs.Matches;
using BetRoyale.API.Entities;
using BetRoyale.API.Enums;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class MatchService : IMatchService
{
    private readonly AppDbContext _dbContext;

    public MatchService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MatchDetailsDto> CreateAsync(CreateMatchRequestDto request, CancellationToken cancellationToken = default)
    {
        var (homeParticipant, awayParticipant) = ValidateAndNormalize(request.HomeParticipant, request.AwayParticipant);
        ValidateRequest(request.Sport, request.Status, request.MatchDate);

        var match = new Match
        {
            Id = Guid.NewGuid(),
            Sport = request.Sport,
            HomeParticipant = homeParticipant,
            AwayParticipant = awayParticipant,
            MatchDate = request.MatchDate,
            Status = request.Status
        };

        _dbContext.Matches.Add(match);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(match);
    }

    public async Task<IReadOnlyList<MatchListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Matches
            .AsNoTracking()
            .OrderBy(match => match.MatchDate)
            .Select(match => new MatchListItemDto
            {
                Id = match.Id,
                Sport = match.Sport,
                HomeParticipant = match.HomeParticipant,
                AwayParticipant = match.AwayParticipant,
                MatchDate = match.MatchDate,
                Status = match.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MatchDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var match = await _dbContext.Matches
            .AsNoTracking()
            .SingleOrDefaultAsync(currentMatch => currentMatch.Id == id, cancellationToken);

        if (match is null)
        {
            throw new MatchNotFoundException($"Match '{id}' was not found.");
        }

        return MapToDetailsDto(match);
    }

    public async Task<MatchDetailsDto> UpdateAsync(Guid id, UpdateMatchRequestDto request, CancellationToken cancellationToken = default)
    {
        var match = await _dbContext.Matches
            .SingleOrDefaultAsync(currentMatch => currentMatch.Id == id, cancellationToken);

        if (match is null)
        {
            throw new MatchNotFoundException($"Match '{id}' was not found.");
        }

        var (homeParticipant, awayParticipant) = ValidateAndNormalize(request.HomeParticipant, request.AwayParticipant);
        ValidateRequest(request.Sport, request.Status, request.MatchDate);

        match.Sport = request.Sport;
        match.HomeParticipant = homeParticipant;
        match.AwayParticipant = awayParticipant;
        match.MatchDate = request.MatchDate;
        match.Status = request.Status;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(match);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var match = await _dbContext.Matches
            .SingleOrDefaultAsync(currentMatch => currentMatch.Id == id, cancellationToken);

        if (match is null)
        {
            throw new MatchNotFoundException($"Match '{id}' was not found.");
        }

        _dbContext.Matches.Remove(match);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateRequest(SportType sport, MatchStatus status, DateTime matchDate)
    {
        if (!Enum.IsDefined(sport))
        {
            throw new InvalidMatchException("Sport value is invalid.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new InvalidMatchException("Match status value is invalid.");
        }

        if (matchDate == default)
        {
            throw new InvalidMatchException("Match date is required.");
        }

        if (matchDate.Kind != DateTimeKind.Utc)
        {
            throw new InvalidMatchException("Match date must be provided in UTC.");
        }
    }

    private static (string HomeParticipant, string AwayParticipant) ValidateAndNormalize(
        string? homeParticipant,
        string? awayParticipant)
    {
        var normalizedHomeParticipant = homeParticipant?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedHomeParticipant))
        {
            throw new InvalidMatchException("Home participant is required.");
        }

        var normalizedAwayParticipant = awayParticipant?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedAwayParticipant))
        {
            throw new InvalidMatchException("Away participant is required.");
        }

        if (string.Equals(normalizedHomeParticipant, normalizedAwayParticipant, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidMatchException("Home participant and away participant must be different.");
        }

        return (normalizedHomeParticipant, normalizedAwayParticipant);
    }

    private static MatchDetailsDto MapToDetailsDto(Match match)
    {
        return new MatchDetailsDto
        {
            Id = match.Id,
            Sport = match.Sport,
            HomeParticipant = match.HomeParticipant,
            AwayParticipant = match.AwayParticipant,
            MatchDate = match.MatchDate,
            Status = match.Status
        };
    }
}
