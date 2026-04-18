using BetRoyale.API.Entities;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BetRoyale.API.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
