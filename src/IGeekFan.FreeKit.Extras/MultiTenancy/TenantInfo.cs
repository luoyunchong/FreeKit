namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class TenantInfo
{
    public Guid? Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Code { get; set; }
    
    public string? ConnectionString { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    public static TenantInfo? Empty => null;
    
    public static TenantInfo Default => new() { Id = null, Name = "Default", Code = "default" };

    public override string ToString()
    {
        return $"Tenant: {Name} ({Id})";
    }
}
