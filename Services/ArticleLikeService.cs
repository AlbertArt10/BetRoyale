using BetRoyale.API.Data;
using BetRoyale.API.DTOs.ArticleLikes;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class ArticleLikeService : IArticleLikeService
{
    private readonly AppDbContext _dbContext;

    public ArticleLikeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ArticleLikeSummaryDto> LikeAsync(
        ArticleLikeRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var articleId = ValidateArticleId(request.ArticleId);

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new ArticleLikeUserNotFoundException($"Article like user '{userId}' was not found.");
        }

        var articleExists = await _dbContext.Articles
            .AsNoTracking()
            .AnyAsync(article => article.Id == articleId, cancellationToken);

        if (!articleExists)
        {
            throw new ArticleNotFoundException($"Article '{articleId}' was not found.");
        }

        var likeExists = await _dbContext.ArticleLikes
            .AsNoTracking()
            .AnyAsync(articleLike => articleLike.ArticleId == articleId && articleLike.UserId == userId, cancellationToken);

        if (likeExists)
        {
            throw new InvalidArticleLikeException("User has already liked this article.");
        }

        var articleLike = new ArticleLike
        {
            ArticleId = articleId,
            UserId = userId
        };

        _dbContext.ArticleLikes.Add(articleLike);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await BuildSummaryAsync(articleId, userId, cancellationToken);
    }

    public async Task<ArticleLikeSummaryDto> UnlikeAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var normalizedArticleId = ValidateArticleId(articleId);

        var articleLike = await _dbContext.ArticleLikes
            .SingleOrDefaultAsync(
                currentArticleLike => currentArticleLike.ArticleId == normalizedArticleId && currentArticleLike.UserId == userId,
                cancellationToken);

        if (articleLike is null)
        {
            throw new ArticleLikeNotFoundException(
                $"Like for article '{normalizedArticleId}' by user '{userId}' was not found.");
        }

        _dbContext.ArticleLikes.Remove(articleLike);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await BuildSummaryAsync(normalizedArticleId, userId, cancellationToken);
    }

    public async Task<ArticleLikeSummaryDto> GetSummaryAsync(
        Guid articleId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedArticleId = ValidateArticleId(articleId);

        var articleExists = await _dbContext.Articles
            .AsNoTracking()
            .AnyAsync(article => article.Id == normalizedArticleId, cancellationToken);

        if (!articleExists)
        {
            throw new ArticleNotFoundException($"Article '{normalizedArticleId}' was not found.");
        }

        return await BuildSummaryAsync(normalizedArticleId, currentUserId, cancellationToken);
    }

    private async Task<ArticleLikeSummaryDto> BuildSummaryAsync(
        Guid articleId,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        var likesCount = await _dbContext.ArticleLikes
            .AsNoTracking()
            .CountAsync(articleLike => articleLike.ArticleId == articleId, cancellationToken);

        var isLikedByCurrentUser = currentUserId.HasValue && await _dbContext.ArticleLikes
            .AsNoTracking()
            .AnyAsync(
                articleLike => articleLike.ArticleId == articleId && articleLike.UserId == currentUserId.Value,
                cancellationToken);

        return new ArticleLikeSummaryDto
        {
            ArticleId = articleId,
            LikesCount = likesCount,
            IsLikedByCurrentUser = isLikedByCurrentUser
        };
    }

    private static Guid ValidateArticleId(Guid articleId)
    {
        if (articleId == Guid.Empty)
        {
            throw new InvalidArticleLikeException("ArticleId is required.");
        }

        return articleId;
    }
}
