using BetRoyale.API.Enums;

namespace BetRoyale.API.DTOs.Matches;

public class MatchDetailsDto
{
    public Guid Id { get; set; }

    public SportType Sport { get; set; }

    public string HomeParticipant { get; set; } = string.Empty;

    public string AwayParticipant { get; set; } = string.Empty;

    public DateTime MatchDate { get; set; }

    public MatchStatus Status { get; set; }
}
