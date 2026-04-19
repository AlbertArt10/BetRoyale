using BetRoyale.API.DTOs.Comments;

namespace BetRoyale.API.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDetailsDto> CreateAsync(
        CreateCommentRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CommentListItemDto>> GetByArticleIdAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    Task<CommentDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CommentDetailsDto> UpdateAsync(
        Guid id,
        UpdateCommentRequestDto request,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
