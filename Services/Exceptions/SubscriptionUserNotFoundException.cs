namespace BetRoyale.API.Services.Exceptions;

public class SubscriptionUserNotFoundException : Exception
{
    public SubscriptionUserNotFoundException(string message)
        : base(message)
    {
    }
}
