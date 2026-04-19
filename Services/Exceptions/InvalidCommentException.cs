namespace BetRoyale.API.Services.Exceptions;

public class InvalidCommentException : Exception
{
    public InvalidCommentException(string message)
        : base(message)
    {
    }
}
