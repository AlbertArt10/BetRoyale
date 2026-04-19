namespace BetRoyale.API.Services.Exceptions;

public class PredictionNotFoundException : Exception
{
    public PredictionNotFoundException(string message)
        : base(message)
    {
    }
}
