namespace BetRoyale.API.DTOs.Subscriptions;

public class SubscriberListItemDto
{
    public Guid SubscriptionId { get; set; }

    public Guid SubscriberId { get; set; }

    public string SubscriberUsername { get; set; } = string.Empty;
}
