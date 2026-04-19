namespace BetRoyale.API.Services.Exceptions;

public class ArticleLikeNotFoundException : Exception
{
    public ArticleLikeNotFoundException(string message)
        : base(message)
    {
    }
}
