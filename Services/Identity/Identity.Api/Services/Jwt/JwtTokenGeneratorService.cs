using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Api.Interfaces.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Services.Jwt;

public class JwtTokenGeneratorService : IJwtTokenGeneratorService
{
    public string GenerateToken(string key, string issuer, string audience, double expires, IEnumerable<Claim>? claims = null)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddMinutes(expires), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}