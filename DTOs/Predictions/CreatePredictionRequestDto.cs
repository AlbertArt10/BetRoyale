using BetRoyale.API.Enums;

namespace BetRoyale.API.DTOs.Predictions;

public class CreatePredictionRequestDto
{
    public Guid MatchId { get; set; }

    public PredictionOutcome Outcome { get; set; }
}
