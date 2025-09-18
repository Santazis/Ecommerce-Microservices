using System.Security.Claims;
using Database;

namespace Identity.Api.Interfaces.Jwt;

public interface IAccessTokenService
{
    string GenerateAccessToken(ApplicationUser user,IList<string> roles);
}