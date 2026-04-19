namespace BetRoyale.API.Services.Exceptions;

public class CommentAuthorNotFoundException : Exception
{
    public CommentAuthorNotFoundException(string message)
        : base(message)
    {
    }
}
