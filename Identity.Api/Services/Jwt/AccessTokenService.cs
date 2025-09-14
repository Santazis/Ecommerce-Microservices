using System.Security.Claims;
using Database;
using Identity.Api.Interfaces.Jwt;
using Identity.Api.Options;

namespace Identity.Api.Services.Jwt;

public class AccessTokenService : IAccessTokenService
{
    private readonly IJwtTokenGeneratorService _jwtTokenGeneratorService;
    private readonly JwtSettings _jwtSettings;

    public AccessTokenService(IJwtTokenGeneratorService jwtTokenGeneratorService, JwtSettings jwtSettings)
    {
        _jwtTokenGeneratorService = jwtTokenGeneratorService;
        _jwtSettings = jwtSettings;
    }
    
    public string GenerateAccessToken(ApplicationUser user,IList<string> roles)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("isEmailConfirmed", user.EmailConfirmed.ToString()),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return _jwtTokenGeneratorService.GenerateToken(_jwtSettings.AccessTokenSecret, _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.AccessTokenExpMinutes, claims);
    }
}