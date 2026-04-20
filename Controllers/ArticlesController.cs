using BetRoyale.API.DTOs.Articles;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpPost]
    [Authorize(Roles = "Analyst,Admin")]
    [ProducesResponseType(typeof(ArticleDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDetailsDto>> Create(
        [FromBody] CreateArticleRequestDto request,
        CancellationToken cancellationToken)
    {
        var authorIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(authorIdClaim, out var authorId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var response = await _articleService.CreateAsync(request, authorId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ArticleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ArticleListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _articleService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Analyst,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var isAdmin = User.IsInRole("Admin");

        await _articleService.DeleteAsync(id, currentUserId, isAdmin, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Analyst,Admin")]
    [ProducesResponseType(typeof(ArticleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDetailsDto>> Update(
        Guid id,
        [FromBody] UpdateArticleRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var isAdmin = User.IsInRole("Admin");

        var response = await _articleService.UpdateAsync(id, request, currentUserId, isAdmin, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ArticleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _articleService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-match/{matchId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ArticleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ArticleListItemDto>>> GetByMatch(Guid matchId, CancellationToken cancellationToken)
    {
        var response = await _articleService.GetByMatchIdAsync(matchId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-author/{authorId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ArticleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ArticleListItemDto>>> GetByAuthor(Guid authorId, CancellationToken cancellationToken)
    {
        var response = await _articleService.GetByAuthorIdAsync(authorId, cancellationToken);
        return Ok(response);
    }
}
