namespace BetRoyale.API.Services.Exceptions;

public class InvalidMatchException : Exception
{
    public InvalidMatchException(string message)
        : base(message)
    {
    }
}
