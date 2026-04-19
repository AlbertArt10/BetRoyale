namespace BetRoyale.API.Services.Exceptions;

public class InvalidPredictionException : Exception
{
    public InvalidPredictionException(string message)
        : base(message)
    {
    }
}
