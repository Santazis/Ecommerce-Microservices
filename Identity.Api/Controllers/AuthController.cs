using Database;
using Identity.Api.Interfaces.Jwt;
using Identity.Api.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccessTokenService _accessTokenService;
    private readonly ApplicationDbContext _applicationDbContext;
    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, SignInManager<ApplicationUser> signInManager, IAccessTokenService accessTokenService, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _signInManager = signInManager;
        _accessTokenService = accessTokenService;
        _applicationDbContext = applicationDbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
        };
        await using var trnsaction = await _applicationDbContext.Database.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        var addToRole = await _userManager.AddToRoleAsync(user, Roles.Customer);
        trnsaction.Commit();
        return Ok();
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return BadRequest();
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, password,false);
        if (!result.Succeeded)
        {
            return BadRequest();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var token = _accessTokenService.GenerateAccessToken(user,roles);
        var response = new LoginResponseDto(token);
        return Ok(response);
    }
}