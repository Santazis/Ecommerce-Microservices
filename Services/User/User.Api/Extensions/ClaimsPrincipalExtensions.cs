using System.Security.Claims;

namespace User.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) return null;
        foreach (var principalIdentity in principal.Identities)
        {
            Console.WriteLine(principalIdentity.Name);
        }
        foreach (var principalClaim in principal.Claims)
        {
            Console.WriteLine(principalClaim.Value);
        }
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            Console.WriteLine("empty claim");
        }
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}