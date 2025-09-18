using Microsoft.AspNetCore.Mvc;
using User.Api.Extensions;
using User.Application.Interfaces;
using User.Application.Models.Requests;

namespace User.Api.Controllers;
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    public Guid? UserId => User.GetUserId();
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfileAsync([FromBody] UpdateProfileRequest request, CancellationToken cancellation)
    {
         if (UserId is null) return Unauthorized();
        var result = await _userService.UpdateProfileAsync(UserId.Value, request, cancellation);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok();
    }
}