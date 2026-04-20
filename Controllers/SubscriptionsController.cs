using BetRoyale.API.DTOs.Subscriptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SubscriptionDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDetailsDto>> Subscribe(
        [FromBody] SubscriptionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var subscriberId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var response = await _subscriptionService.SubscribeAsync(request, subscriberId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{analystId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unsubscribe(Guid analystId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var subscriberId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        await _subscriptionService.UnsubscribeAsync(analystId, subscriberId, cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<SubscribedAnalystListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<SubscribedAnalystListItemDto>>> GetMySubscriptions(
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var subscriberId))
        {
            return Unauthorized(new { message = "Invalid authenticated user." });
        }

        var response = await _subscriptionService.GetMySubscriptionsAsync(subscriberId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("subscribers/{analystId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<SubscriberListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<SubscriberListItemDto>>> GetSubscribersByAnalystId(
        Guid analystId,
        CancellationToken cancellationToken)
    {
        var response = await _subscriptionService.GetSubscribersByAnalystIdAsync(analystId, cancellationToken);
        return Ok(response);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out userId);
    }
}
