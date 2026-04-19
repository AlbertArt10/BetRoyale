using BetRoyale.API.Data;
using BetRoyale.API.DTOs.Comments;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _dbContext;

    public CommentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommentDetailsDto> CreateAsync(
        CreateCommentRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var (content, articleId) = ValidateCreateRequest(request.Content, request.ArticleId);

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(currentUser => currentUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new CommentAuthorNotFoundException($"Comment author '{userId}' was not found.");
        }

        var articleExists = await _dbContext.Articles
            .AsNoTracking()
            .AnyAsync(article => article.Id == articleId, cancellationToken);

        if (!articleExists)
        {
            throw new ArticleNotFoundException($"Article '{articleId}' was not found.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = content,
            UserId = userId,
            ArticleId = articleId
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(comment, user.Username);
    }

    public async Task<IReadOnlyList<CommentListItemDto>> GetByArticleIdAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.ArticleId == articleId)
            .OrderBy(comment => comment.Id)
            .Select(comment => new CommentListItemDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                Username = comment.User.Username,
                ArticleId = comment.ArticleId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CommentDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _dbContext.Comments
            .AsNoTracking()
            .Where(currentComment => currentComment.Id == id)
            .Select(currentComment => new CommentDetailsDto
            {
                Id = currentComment.Id,
                Content = currentComment.Content,
                UserId = currentComment.UserId,
                Username = currentComment.User.Username,
                ArticleId = currentComment.ArticleId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            throw new CommentNotFoundException($"Comment '{id}' was not found.");
        }

        return comment;
    }

    public async Task<CommentDetailsDto> UpdateAsync(
        Guid id,
        UpdateCommentRequestDto request,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var comment = await _dbContext.Comments
            .Include(currentComment => currentComment.User)
            .SingleOrDefaultAsync(currentComment => currentComment.Id == id, cancellationToken);

        if (comment is null)
        {
            throw new CommentNotFoundException($"Comment '{id}' was not found.");
        }

        if (!isAdmin && comment.UserId != currentUserId)
        {
            throw new CommentUpdateForbiddenException("Only the comment author can update this comment.");
        }

        comment.Content = ValidateContent(request.Content);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(comment, comment.User.Username);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var comment = await _dbContext.Comments
            .SingleOrDefaultAsync(currentComment => currentComment.Id == id, cancellationToken);

        if (comment is null)
        {
            throw new CommentNotFoundException($"Comment '{id}' was not found.");
        }

        if (!isAdmin && comment.UserId != currentUserId)
        {
            throw new CommentUpdateForbiddenException("Only the comment author can delete this comment.");
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static (string Content, Guid ArticleId) ValidateCreateRequest(string? content, Guid articleId)
    {
        var normalizedContent = ValidateContent(content);

        if (articleId == Guid.Empty)
        {
            throw new InvalidCommentException("ArticleId is required.");
        }

        return (normalizedContent, articleId);
    }

    private static string ValidateContent(string? content)
    {
        var normalizedContent = content?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedContent))
        {
            throw new InvalidCommentException("Content is required.");
        }

        return normalizedContent;
    }

    private static CommentDetailsDto MapToDetailsDto(Comment comment, string username)
    {
        return new CommentDetailsDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            Username = username,
            ArticleId = comment.ArticleId
        };
    }
}
