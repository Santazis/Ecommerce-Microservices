using Database;
using FluentValidation;
using Identity.Api.Interfaces;
using Identity.Api.Interfaces.Jwt;
using Identity.Api.Models.Dtos;
using Identity.Api.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccessTokenService _accessTokenService;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IUserServiceClient _userService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, SignInManager<ApplicationUser> signInManager, IAccessTokenService accessTokenService, ApplicationDbContext applicationDbContext, IUserServiceClient userService, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator, ILogger<AuthController> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _dbContext = dbContext;
        _signInManager = signInManager;
        _accessTokenService = accessTokenService;
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody]RegisterRequest request,CancellationToken cancellation)
    {
        await _registerValidator.ValidateAndThrowAsync(request,cancellation);
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
        };
        await using var trnsaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellation);
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        var addToRole = await _userManager.AddToRoleAsync(user, Roles.Customer);
        if (!addToRole.Succeeded)
        {
            return BadRequest(addToRole.Errors);
        }
        var createProfile = await _userService.CreateProfileAsync(user.Id,user.Email,cancellation);
        await trnsaction.CommitAsync(cancellation);
        return Ok(createProfile);
    }
    [ProducesResponseType<LoginResponseDto>(200)]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request)
    {
        _logger.LogInformation("Login request received");
        var user = await _userManager.FindByEmailAsync(request.Email);;
        if (user is null)
        {
            return BadRequest();
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,false);
        if (!result.Succeeded)
        {
            return BadRequest();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var token = _accessTokenService.GenerateAccessToken(user,roles);
        var response = new LoginResponseDto(token);
        return Ok(response);
    }
   
    
    [HttpGet("test-auth")]
    public async Task<IActionResult> TestAuth(CancellationToken cancellation)
    {   
        var createProfile = await _userService.CreateProfileAsync(Guid.NewGuid(), "EMAil",cancellation);

        return Ok(createProfile);
    }
}