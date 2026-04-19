namespace BetRoyale.API.Services.Exceptions;

public class InvalidUserRoleChangeException : Exception
{
    public InvalidUserRoleChangeException(string message)
        : base(message)
    {
    }
}
