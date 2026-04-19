using BetRoyale.API.Enums;

namespace BetRoyale.API.Entities;

public class Match
{
    public Guid Id { get; set; }

    public SportType Sport { get; set; }

    public string HomeParticipant { get; set; } = string.Empty;

    public string AwayParticipant { get; set; } = string.Empty;

    public DateTime MatchDate { get; set; }

    public MatchStatus Status { get; set; }

    public int? HomeScore { get; set; }

    public int? AwayScore { get; set; }

    public ICollection<Article> Articles { get; set; } = new List<Article>();

    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}
