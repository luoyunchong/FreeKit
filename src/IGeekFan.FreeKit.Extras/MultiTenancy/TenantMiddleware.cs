using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<ITenantResolver> _tenantResolvers;
    private readonly ITenantAccessor _tenantAccessor;

    public TenantMiddleware(
        RequestDelegate next,
        IEnumerable<ITenantResolver> tenantResolvers,
        ITenantAccessor tenantAccessor)
    {
        _next = next;
        _tenantResolvers = tenantResolvers.OrderByDescending(r => r.Priority);
        _tenantAccessor = tenantAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldIgnoreTenant(context))
        {
            await _next(context);
            return;
        }

        TenantInfo? tenant = null;

        foreach (var resolver in _tenantResolvers)
        {
            tenant = await resolver.ResolveAsync(context);
            if (tenant != null)
            {
                break;
            }
        }

        _tenantAccessor.Tenant = tenant;

        await _next(context);
    }

    private static bool ShouldIgnoreTenant(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null) return false;

        if (endpoint.Metadata.GetMetadata<IgnoreTenantAttribute>() != null)
        {
            return true;
        }

        var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor == null) return false;

        if (actionDescriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreTenantAttribute), true).Any())
        {
            return true;
        }

        if (actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(IgnoreTenantAttribute), true).Any())
        {
            return true;
        }

        return false;
    }
}
