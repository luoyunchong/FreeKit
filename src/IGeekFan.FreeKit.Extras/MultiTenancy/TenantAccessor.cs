namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class TenantAccessor : ITenantAccessor
{
    private static readonly AsyncLocal<TenantInfo?> _currentTenant = new();

    public TenantInfo? Tenant
    {
        get => _currentTenant.Value;
        set => _currentTenant.Value = value;
    }

    public static TenantInfo? CurrentTenant => _currentTenant.Value;
}
