using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");

        builder.HasKey(match => match.Id);

        builder.Property(match => match.Sport)
            .IsRequired();

        builder.Property(match => match.HomeParticipant)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(match => match.AwayParticipant)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(match => match.MatchDate)
            .IsRequired();

        builder.Property(match => match.Status)
            .IsRequired();

        builder.Property(match => match.HomeScore);

        builder.Property(match => match.AwayScore);
    }
}
