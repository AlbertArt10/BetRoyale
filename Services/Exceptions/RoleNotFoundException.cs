namespace BetRoyale.API.Services.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string message)
        : base(message)
    {
    }
}
