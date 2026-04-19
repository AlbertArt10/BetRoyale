namespace BetRoyale.API.Services.Exceptions;

public class InvalidArticleLikeException : Exception
{
    public InvalidArticleLikeException(string message)
        : base(message)
    {
    }
}
