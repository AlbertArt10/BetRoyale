using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(subscription => subscription.Id);

        builder.HasIndex(subscription => new { subscription.SubscriberId, subscription.AnalystId })
            .IsUnique();

        builder.HasOne(subscription => subscription.Subscriber)
            .WithMany(user => user.Subscriptions)
            .HasForeignKey(subscription => subscription.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(subscription => subscription.Analyst)
            .WithMany(user => user.Subscribers)
            .HasForeignKey(subscription => subscription.AnalystId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
