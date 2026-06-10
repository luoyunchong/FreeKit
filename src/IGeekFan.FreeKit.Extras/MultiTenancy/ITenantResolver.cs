using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public interface ITenantResolver
{
    Task<TenantInfo?> ResolveAsync(HttpContext context);
    
    int Priority { get; }
}
