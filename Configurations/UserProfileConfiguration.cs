using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.FullName)
            .HasMaxLength(150);

        builder.Property(profile => profile.Bio)
            .HasMaxLength(1000);

        builder.Property(profile => profile.TotalPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(profile => profile.UserId)
            .IsUnique();

        builder.HasOne(profile => profile.User)
            .WithOne(user => user.UserProfile)
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
