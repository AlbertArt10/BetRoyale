using BetRoyale.API.DTOs.Comments;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CommentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDetailsDto>> Create(
        [FromBody] CreateCommentRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        try
        {
            var response = await _commentService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidCommentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (CommentAuthorNotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArticleNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CommentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _commentService.GetByIdAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("by-article/{articleId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CommentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommentListItemDto>>> GetByArticle(
        Guid articleId,
        CancellationToken cancellationToken)
    {
        var response = await _commentService.GetByArticleIdAsync(articleId, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(CommentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDetailsDto>> Update(
        Guid id,
        [FromBody] UpdateCommentRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var response = await _commentService.UpdateAsync(id, request, currentUserId, isAdmin, cancellationToken);
            return Ok(response);
        }
        catch (InvalidCommentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (CommentUpdateForbiddenException)
        {
            return Forbid();
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _commentService.DeleteAsync(id, currentUserId, isAdmin, cancellationToken);
            return NoContent();
        }
        catch (CommentUpdateForbiddenException)
        {
            return Forbid();
        }
        catch (CommentNotFoundException ex)
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
