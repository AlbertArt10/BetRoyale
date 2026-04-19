namespace BetRoyale.API.Services.Exceptions;

public class PredictionUserNotFoundException : Exception
{
    public PredictionUserNotFoundException(string message)
        : base(message)
    {
    }
}
