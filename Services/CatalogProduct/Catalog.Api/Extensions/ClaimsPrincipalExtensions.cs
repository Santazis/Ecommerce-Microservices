using System.Security.Claims;

namespace Catalog.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) Console.WriteLine("empty claim");
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}