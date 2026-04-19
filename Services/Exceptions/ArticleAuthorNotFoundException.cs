namespace BetRoyale.API.Services.Exceptions;

public class ArticleAuthorNotFoundException : Exception
{
    public ArticleAuthorNotFoundException(string message)
        : base(message)
    {
    }
}
