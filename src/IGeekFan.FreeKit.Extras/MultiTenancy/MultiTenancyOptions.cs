namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class MultiTenancyOptions
{
    public bool IsEnabled { get; set; } = true;
    
    public bool EnableHeaderResolver { get; set; } = true;
    
    public string HeaderName { get; set; } = "X-Tenant-Id";
    
    public bool EnableQueryStringResolver { get; set; } = true;
    
    public string QueryStringKey { get; set; } = "tenantId";
    
    public bool EnableClaimResolver { get; set; } = true;
    
    public bool EnableHostResolver { get; set; } = false;
    
    public string? TenantDomainSuffix { get; set; }
    
    public bool UseSubdomain { get; set; } = true;
    
    public bool EnableGlobalQueryFilter { get; set; } = true;
}
