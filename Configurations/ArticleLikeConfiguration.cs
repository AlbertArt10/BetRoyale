using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class ArticleLikeConfiguration : IEntityTypeConfiguration<ArticleLike>
{
    public void Configure(EntityTypeBuilder<ArticleLike> builder)
    {
        builder.ToTable("ArticleLikes");

        builder.HasKey(articleLike => new { articleLike.UserId, articleLike.ArticleId });

        builder.HasOne(articleLike => articleLike.User)
            .WithMany(user => user.ArticleLikes)
            .HasForeignKey(articleLike => articleLike.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(articleLike => articleLike.Article)
            .WithMany(article => article.ArticleLikes)
            .HasForeignKey(articleLike => articleLike.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(articleLike => articleLike.ArticleId);
    }
}
