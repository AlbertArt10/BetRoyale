namespace BetRoyale.API.Services.Exceptions;

public class ArticleUpdateForbiddenException : Exception
{
    public ArticleUpdateForbiddenException(string message)
        : base(message)
    {
    }
}
