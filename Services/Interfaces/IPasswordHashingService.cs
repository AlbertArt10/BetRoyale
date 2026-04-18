using BetRoyale.API.Entities;

namespace BetRoyale.API.Services.Interfaces;

public interface IPasswordHashingService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}
