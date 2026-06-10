using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class HeaderTenantResolver : ITenantResolver
{
    private readonly string _headerName;

    public HeaderTenantResolver(string headerName = "X-Tenant-Id")
    {
        _headerName = headerName;
    }

    public int Priority => 20;

    public Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_headerName, out var tenantIdValue))
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        var tenantIdStr = tenantIdValue.ToString();
        if (string.IsNullOrEmpty(tenantIdStr))
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        if (Guid.TryParse(tenantIdStr, out var tenantId))
        {
            return Task.FromResult<TenantInfo?>(new TenantInfo { Id = tenantId });
        }

        return Task.FromResult<TenantInfo?>(new TenantInfo { Code = tenantIdStr });
    }
}
