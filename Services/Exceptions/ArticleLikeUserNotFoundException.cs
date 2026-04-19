namespace BetRoyale.API.Services.Exceptions;

public class ArticleLikeUserNotFoundException : Exception
{
    public ArticleLikeUserNotFoundException(string message)
        : base(message)
    {
    }
}
