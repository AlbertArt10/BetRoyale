using BetRoyale.API.DTOs.Admin;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(IReadOnlyList<UserRoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserRoleResponseDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var response = await _adminService.GetUsersAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(AdminUserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserDetailsDto>> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _adminService.GetUserByIdAsync(userId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("users")]
    [ProducesResponseType(typeof(AdminUserDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserDetailsDto>> CreateUser(
        [FromBody] CreateAdminUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _adminService.CreateUserAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { userId = response.UserId }, response);
    }

    [HttpPut("users/{userId:guid}")]
    [ProducesResponseType(typeof(AdminUserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserDetailsDto>> UpdateUser(
        Guid userId,
        [FromBody] UpdateAdminUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _adminService.UpdateUserAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await _adminService.DeleteUserAsync(userId, cancellationToken);
        return NoContent();
    }
}
