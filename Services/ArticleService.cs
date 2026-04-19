using BetRoyale.API.Data;
using BetRoyale.API.DTOs.Articles;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class ArticleService : IArticleService
{
    private readonly AppDbContext _dbContext;

    public ArticleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ArticleDetailsDto> CreateAsync(
        CreateArticleRequestDto request,
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        var (title, content, matchId) = ValidateRequest(request.Title, request.Content, request.MatchId);

        var author = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == authorId, cancellationToken);

        if (author is null)
        {
            throw new ArticleAuthorNotFoundException($"Article author '{authorId}' was not found.");
        }

        var matchExists = await _dbContext.Matches
            .AsNoTracking()
            .AnyAsync(match => match.Id == matchId, cancellationToken);

        if (!matchExists)
        {
            throw new MatchNotFoundException($"Match '{matchId}' was not found.");
        }

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            AuthorId = authorId,
            MatchId = matchId
        };

        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ArticleDetailsDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            AuthorId = article.AuthorId,
            AuthorUsername = author.Username,
            MatchId = article.MatchId
        };
    }

    public async Task<ArticleDetailsDto> UpdateAsync(
        Guid id,
        UpdateArticleRequestDto request,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles
            .Include(currentArticle => currentArticle.Author)
            .SingleOrDefaultAsync(currentArticle => currentArticle.Id == id, cancellationToken);

        if (article is null)
        {
            throw new ArticleNotFoundException($"Article '{id}' was not found.");
        }

        if (!isAdmin && article.AuthorId != currentUserId)
        {
            throw new ArticleUpdateForbiddenException("Only the article author can update this article.");
        }

        var (title, content, matchId) = ValidateRequest(request.Title, request.Content, request.MatchId);

        var matchExists = await _dbContext.Matches
            .AsNoTracking()
            .AnyAsync(match => match.Id == matchId, cancellationToken);

        if (!matchExists)
        {
            throw new MatchNotFoundException($"Match '{matchId}' was not found.");
        }

        article.Title = title;
        article.Content = content;
        article.MatchId = matchId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ArticleDetailsDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            AuthorId = article.AuthorId,
            AuthorUsername = article.Author.Username,
            MatchId = article.MatchId
        };
    }

    public async Task DeleteAsync(
        Guid id,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles
            .SingleOrDefaultAsync(currentArticle => currentArticle.Id == id, cancellationToken);

        if (article is null)
        {
            throw new ArticleNotFoundException($"Article '{id}' was not found.");
        }

        if (!isAdmin && article.AuthorId != currentUserId)
        {
            throw new ArticleUpdateForbiddenException("Only the article author can delete this article.");
        }

        _dbContext.Articles.Remove(article);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ArticleListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Articles
            .AsNoTracking()
            .OrderBy(article => article.Title)
            .Select(article => new ArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                AuthorId = article.AuthorId,
                AuthorUsername = article.Author.Username,
                MatchId = article.MatchId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ArticleDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles
            .AsNoTracking()
            .Where(currentArticle => currentArticle.Id == id)
            .Select(currentArticle => new ArticleDetailsDto
            {
                Id = currentArticle.Id,
                Title = currentArticle.Title,
                Content = currentArticle.Content,
                AuthorId = currentArticle.AuthorId,
                AuthorUsername = currentArticle.Author.Username,
                MatchId = currentArticle.MatchId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (article is null)
        {
            throw new ArticleNotFoundException($"Article '{id}' was not found.");
        }

        return article;
    }

    public async Task<IReadOnlyList<ArticleListItemDto>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Articles
            .AsNoTracking()
            .Where(article => article.MatchId == matchId)
            .OrderBy(article => article.Title)
            .Select(article => new ArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                AuthorId = article.AuthorId,
                AuthorUsername = article.Author.Username,
                MatchId = article.MatchId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ArticleListItemDto>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Articles
            .AsNoTracking()
            .Where(article => article.AuthorId == authorId)
            .OrderBy(article => article.Title)
            .Select(article => new ArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                AuthorId = article.AuthorId,
                AuthorUsername = article.Author.Username,
                MatchId = article.MatchId
            })
            .ToListAsync(cancellationToken);
    }

    private static (string Title, string Content, Guid MatchId) ValidateRequest(
        string? title,
        string? content,
        Guid matchId)
    {
        var normalizedTitle = title?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            throw new InvalidArticleException("Title is required.");
        }

        var normalizedContent = content?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedContent))
        {
            throw new InvalidArticleException("Content is required.");
        }

        if (matchId == Guid.Empty)
        {
            throw new InvalidArticleException("MatchId is required.");
        }

        return (normalizedTitle, normalizedContent, matchId);
    }
}
