using BetRoyale.API.Entities;

namespace BetRoyale.API.Data.Seed;

public static class RoleSeedData
{
    public static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AnalystId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid UserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Role Admin = new()
    {
        Id = AdminId,
        Name = "Admin"
    };

    public static readonly Role Analyst = new()
    {
        Id = AnalystId,
        Name = "Analyst"
    };

    public static readonly Role User = new()
    {
        Id = UserId,
        Name = "User"
    };
}
