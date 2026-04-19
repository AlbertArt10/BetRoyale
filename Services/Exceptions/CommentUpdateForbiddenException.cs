namespace BetRoyale.API.Services.Exceptions;

public class CommentUpdateForbiddenException : Exception
{
    public CommentUpdateForbiddenException(string message)
        : base(message)
    {
    }
}
