
using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public static class FreeKitClaimTypes
{
    public const string TenantName = "tenantname";
    public const string TenantId = "tenantid";
    public const string NameIdentifier = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string Email = ClaimTypes.Email;
    public const string Name = ClaimTypes.GivenName;
    public const string Role = ClaimTypes.Role;
}