using BetRoyale.API.DTOs.ArticleLikes;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleLikesController : ControllerBase
{
    private readonly IArticleLikeService _articleLikeService;

    public ArticleLikesController(IArticleLikeService articleLikeService)
    {
        _articleLikeService = articleLikeService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ArticleLikeSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleLikeSummaryDto>> Like(
        [FromBody] ArticleLikeRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        try
        {
            var response = await _articleLikeService.LikeAsync(request, userId, cancellationToken);
            return Ok(response);
        }
        catch (InvalidArticleLikeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArticleLikeUserNotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArticleNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{articleId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ArticleLikeSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleLikeSummaryDto>> Unlike(
        Guid articleId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        try
        {
            var response = await _articleLikeService.UnlikeAsync(articleId, userId, cancellationToken);
            return Ok(response);
        }
        catch (InvalidArticleLikeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArticleLikeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("summary/{articleId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ArticleLikeSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleLikeSummaryDto>> GetSummary(
        Guid articleId,
        CancellationToken cancellationToken)
    {
        Guid? currentUserId = TryGetCurrentUserId(out var parsedUserId) ? parsedUserId : null;

        try
        {
            var response = await _articleLikeService.GetSummaryAsync(articleId, currentUserId, cancellationToken);
            return Ok(response);
        }
        catch (ArticleNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out userId);
    }
}
