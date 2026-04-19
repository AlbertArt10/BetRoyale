using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> builder)
    {
        builder.ToTable("Predictions");

        builder.HasKey(prediction => prediction.Id);

        builder.HasIndex(prediction => new { prediction.UserId, prediction.MatchId })
            .IsUnique();

        builder.Property(prediction => prediction.Outcome)
            .IsRequired();

        builder.Property(prediction => prediction.IsCorrect);

        builder.Property(prediction => prediction.PointsAwarded);

        builder.HasOne(prediction => prediction.User)
            .WithMany(user => user.Predictions)
            .HasForeignKey(prediction => prediction.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(prediction => prediction.Match)
            .WithMany(match => match.Predictions)
            .HasForeignKey(prediction => prediction.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
