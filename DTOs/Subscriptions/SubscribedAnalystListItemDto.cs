namespace BetRoyale.API.DTOs.Subscriptions;

public class SubscribedAnalystListItemDto
{
    public Guid SubscriptionId { get; set; }

    public Guid AnalystId { get; set; }

    public string AnalystUsername { get; set; } = string.Empty;
}
