using System.Security.Claims;

namespace Identity.Api.Interfaces.Jwt;

public interface IJwtTokenGeneratorService
{
    string GenerateToken(string key,string issuer,string audience,double expires,IEnumerable<Claim>? claims = null);
}