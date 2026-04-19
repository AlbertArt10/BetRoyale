namespace BetRoyale.API.Services.Exceptions;

public class InvalidArticleException : Exception
{
    public InvalidArticleException(string message)
        : base(message)
    {
    }
}
