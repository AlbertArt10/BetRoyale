namespace BetRoyale.API.Services.Exceptions;

public class SubscriptionNotFoundException : Exception
{
    public SubscriptionNotFoundException(string message)
        : base(message)
    {
    }
}
