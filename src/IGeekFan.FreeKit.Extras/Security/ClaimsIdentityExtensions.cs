using IGeekFan.FreeKit.Extras.Extensions;
using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public static class ClaimsIdentityExtensions
{
 
    public static string? FindUserId(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }
        return userIdOrNull.Value;
    }

    public static string? FindUserName(this ClaimsPrincipal principal)
    {
        Claim? userNameOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        return userNameOrNull?.Value;
    }

}

