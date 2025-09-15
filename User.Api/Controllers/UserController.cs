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
    

        // if (UserId is null) return Unauthorized("User not found");
        var userId = Guid.Parse("01994ea1-fb1e-7e5e-89b7-c3d2958e9a04");
        var result = await _userService.UpdateProfileAsync(userId, request, cancellation);
        
        return Ok();
    }
}