using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<ITenantResolver> _tenantResolvers;
    private readonly ITenantAccessor _tenantAccessor;
    private readonly IOptionsMonitor<MultiTenancyOptions> _options;

    public TenantMiddleware(
        RequestDelegate next,
        IEnumerable<ITenantResolver> tenantResolvers,
        ITenantAccessor tenantAccessor,
        IOptionsMonitor<MultiTenancyOptions> options)
    {
        _next = next;
        _tenantResolvers = tenantResolvers.OrderByDescending(r => r.Priority);
        _tenantAccessor = tenantAccessor;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.CurrentValue.IsEnabled || ShouldIgnoreTenant(context))
        {
            await _next(context);
            return;
        }

        TenantInfo? previousTenant = _tenantAccessor.Tenant;
        TenantInfo? tenant = null;
        try
        {
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
        finally
        {
            // AsyncLocal state must not leak into a nested request or background operation.
            _tenantAccessor.Tenant = previousTenant;
        }
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
