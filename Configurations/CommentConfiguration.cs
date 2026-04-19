using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Content)
            .IsRequired();

        builder.HasOne(comment => comment.User)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(comment => comment.Article)
            .WithMany(article => article.Comments)
            .HasForeignKey(comment => comment.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
