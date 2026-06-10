namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class MultiTenancyException : Exception
{
    public MultiTenancyException(string message) : base(message)
    {
    }

    public MultiTenancyException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public static MultiTenancyException TenantNotFound(Guid tenantId)
        => new($"Tenant with id '{tenantId}' was not found.");

    public static MultiTenancyException TenantNotResolved()
        => new("Could not resolve tenant from the current context.");

    public static MultiTenancyException TenantDisabled(Guid tenantId)
        => new($"Tenant with id '{tenantId}' is disabled.");
}
