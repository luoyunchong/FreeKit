using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class ClaimTenantResolver : ITenantResolver
{
    public int Priority => 10;

    public Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        var tenantId = user.FindTenantId();
        var tenantName = user.Claims.FirstOrDefault(c => c.Type == FreeKitClaimTypes.TenantName)?.Value;

        if (tenantId == null && string.IsNullOrEmpty(tenantName))
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        return Task.FromResult<TenantInfo?>(new TenantInfo
        {
            Id = tenantId,
            Name = tenantName
        });
    }
}
