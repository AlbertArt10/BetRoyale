using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<Match> Matches => Set<Match>();

    public DbSet<Article> Articles => Set<Article>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<ArticleLike> ArticleLikes => Set<ArticleLike>();

    public DbSet<Prediction> Predictions => Set<Prediction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
