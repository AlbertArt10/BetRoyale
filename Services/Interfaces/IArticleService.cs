using BetRoyale.API.DTOs.Articles;

namespace BetRoyale.API.Services.Interfaces;

public interface IArticleService
{
    Task<ArticleDetailsDto> CreateAsync(
        CreateArticleRequestDto request,
        Guid authorId,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailsDto> UpdateAsync(
        Guid id,
        UpdateArticleRequestDto request,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ArticleListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ArticleDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ArticleListItemDto>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ArticleListItemDto>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}
