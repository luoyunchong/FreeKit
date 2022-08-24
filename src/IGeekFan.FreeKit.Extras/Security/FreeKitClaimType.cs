
using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public static class FreeKitClaimType
{
    public const string TenantName = "TenantName";
    public const string TenantId = "TenantId";
    public const string NameIdentifier = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string Email = ClaimTypes.Email;
    public const string Name =ClaimTypes.GivenName;
    public const string Role =ClaimTypes.Role;
}