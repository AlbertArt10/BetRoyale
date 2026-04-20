using BetRoyale.API.DTOs.Matches;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MatchDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MatchDetailsDto>> Create(
        [FromBody] CreateMatchRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _matchService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MatchListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MatchListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _matchService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MatchDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _matchService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MatchDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchDetailsDto>> Update(
        Guid id,
        [FromBody] UpdateMatchRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _matchService.UpdateAsync(id, request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}/result")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MatchDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchDetailsDto>> SetResult(
        Guid id,
        [FromBody] SetMatchResultRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _matchService.SetResultAsync(id, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _matchService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
