using BetRoyale.API.DTOs.ArticleLikes;

namespace BetRoyale.API.Services.Interfaces;

public interface IArticleLikeService
{
    Task<ArticleLikeSummaryDto> LikeAsync(
        ArticleLikeRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ArticleLikeSummaryDto> UnlikeAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ArticleLikeSummaryDto> GetSummaryAsync(
        Guid articleId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
