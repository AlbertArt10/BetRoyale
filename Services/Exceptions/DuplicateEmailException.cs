namespace BetRoyale.API.Services.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string message)
        : base(message)
    {
    }
}
