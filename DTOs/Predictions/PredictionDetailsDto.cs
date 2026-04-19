using BetRoyale.API.Enums;

namespace BetRoyale.API.DTOs.Predictions;

public class PredictionDetailsDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public Guid MatchId { get; set; }

    public SportType Sport { get; set; }

    public PredictionOutcome Outcome { get; set; }

    public bool? IsCorrect { get; set; }

    public int? PointsAwarded { get; set; }
}
