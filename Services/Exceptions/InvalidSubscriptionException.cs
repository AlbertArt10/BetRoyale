namespace BetRoyale.API.Services.Exceptions;

public class InvalidSubscriptionException : Exception
{
    public InvalidSubscriptionException(string message)
        : base(message)
    {
    }
}
