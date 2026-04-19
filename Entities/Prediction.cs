using BetRoyale.API.Enums;

namespace BetRoyale.API.Entities;

public class Prediction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid MatchId { get; set; }

    public Match Match { get; set; } = null!;

    public PredictionOutcome Outcome { get; set; }

    public bool? IsCorrect { get; set; }

    public int? PointsAwarded { get; set; }
}
