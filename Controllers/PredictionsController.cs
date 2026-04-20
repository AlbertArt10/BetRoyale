using BetRoyale.API.DTOs.Predictions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public PredictionsController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PredictionDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PredictionDetailsDto>> Create(
        [FromBody] CreatePredictionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var response = await _predictionService.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PredictionDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PredictionDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _predictionService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-match/{matchId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PredictionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PredictionListItemDto>>> GetByMatch(
        Guid matchId,
        CancellationToken cancellationToken)
    {
        var response = await _predictionService.GetByMatchIdAsync(matchId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<PredictionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<PredictionListItemDto>>> GetMine(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var response = await _predictionService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(response);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out userId);
    }
}
