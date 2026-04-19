using BetRoyale.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetRoyale.API.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(article => article.Id);

        builder.Property(article => article.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(article => article.Content)
            .IsRequired();

        builder.HasOne(article => article.Author)
            .WithMany(user => user.Articles)
            .HasForeignKey(article => article.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(article => article.Match)
            .WithMany(match => match.Articles)
            .HasForeignKey(article => article.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
