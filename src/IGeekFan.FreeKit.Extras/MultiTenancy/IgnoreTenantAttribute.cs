namespace IGeekFan.FreeKit.Extras.MultiTenancy;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class IgnoreTenantAttribute : Attribute
{
}
