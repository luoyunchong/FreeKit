namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public interface ITenantAccessor
{
    TenantInfo? Tenant { get; set; }
    
    Guid? TenantId => Tenant?.Id;
    
    string? TenantName => Tenant?.Name;
    
    bool IsTenantResolved => Tenant != null;
}
