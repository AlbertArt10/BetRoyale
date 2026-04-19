namespace BetRoyale.API.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public Guid SubscriberId { get; set; }

    public Guid AnalystId { get; set; }

    public User Subscriber { get; set; } = null!;

    public User Analyst { get; set; } = null!;
}
