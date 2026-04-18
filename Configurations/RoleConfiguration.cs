using BetRoyale.API.Data.Seed;
using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.HasData(RoleSeedData.Admin, RoleSeedData.Analyst, RoleSeedData.User);
    }
}
