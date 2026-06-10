using IGeekFan.FreeKit.Extras.MultiTenancy;

namespace IGeekFan.FreeKit.Extras.Security;

public static class CurrentUserMultiTenancyExtensions
{
    public static TenantInfo? GetCurrentTenant(this ICurrentUser currentUser, ITenantAccessor tenantAccessor)
    {
        return tenantAccessor.Tenant;
    }
}
