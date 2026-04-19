namespace BetRoyale.API.Services.Exceptions;

public class InvalidProfileException : Exception
{
    public InvalidProfileException(string message)
        : base(message)
    {
    }
}
