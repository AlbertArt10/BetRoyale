namespace BetRoyale.API.Services.Exceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException(string message)
        : base(message)
    {
    }
}
