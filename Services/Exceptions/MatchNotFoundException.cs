namespace BetRoyale.API.Services.Exceptions;

public class MatchNotFoundException : Exception
{
    public MatchNotFoundException(string message)
        : base(message)
    {
    }
}
