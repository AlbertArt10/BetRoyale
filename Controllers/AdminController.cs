using BetRoyale.API.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("ping")]
    [ProducesResponseType(typeof(AdminPingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<AdminPingResponseDto> Ping()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var usernameClaim = User.Identity?.Name
            ?? User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        Guid? userId = Guid.TryParse(userIdClaim, out var parsedUserId)
            ? parsedUserId
            : null;

        return Ok(new AdminPingResponseDto
        {
            Message = "Admin access granted.",
            UserId = userId,
            Username = usernameClaim ?? string.Empty,
            Role = roleClaim ?? string.Empty
        });
    }
}
