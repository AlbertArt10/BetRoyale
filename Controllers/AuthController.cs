using BetRoyale.API.DTOs.Auth;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetRoyale.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidUsernameException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidEmailException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateUsernameException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (RoleNotFoundException ex)
        {
            return Problem(
                title: "Registration failed.",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
