using BetRoyale.API.Entities;

namespace BetRoyale.API.Services.Interfaces;

public interface ITokenService
{
    AuthTokenResult GenerateAccessToken(User user, string roleName);
}
