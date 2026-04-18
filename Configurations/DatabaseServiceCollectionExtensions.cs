using BetRoyale.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Configurations;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing. Configure it via environment variables or user secrets.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
