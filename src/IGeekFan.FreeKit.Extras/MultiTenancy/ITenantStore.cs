using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public interface ITenantStore : ITransientDependency
{
    Task<TenantInfo?> FindByIdAsync(Guid tenantId);
    
    Task<TenantInfo?> FindByNameAsync(string tenantName);
    
    Task<TenantInfo?> FindByCodeAsync(string tenantCode);
}
