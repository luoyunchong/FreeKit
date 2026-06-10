using Microsoft.AspNetCore.Mvc.Filters;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class IgnoreTenantFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var hasIgnoreAttribute = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is IgnoreTenantAttribute);

        if (hasIgnoreAttribute)
        {
            using (FreeSqlMultiTenancyExtensions.BeginIgnoreTenant())
            {
                await next();
            }
        }
        else
        {
            await next();
        }
    }
}
