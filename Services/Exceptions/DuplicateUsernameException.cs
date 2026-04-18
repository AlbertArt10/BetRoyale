namespace BetRoyale.API.Services.Exceptions;

public class DuplicateUsernameException : Exception
{
    public DuplicateUsernameException(string message)
        : base(message)
    {
    }
}
