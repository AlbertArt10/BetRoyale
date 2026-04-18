namespace BetRoyale.API.Services.Exceptions;

public class InvalidUsernameException : Exception
{
    public InvalidUsernameException(string message)
        : base(message)
    {
    }
}
