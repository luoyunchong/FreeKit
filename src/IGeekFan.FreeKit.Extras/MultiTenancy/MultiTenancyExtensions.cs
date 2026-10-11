using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public static class MultiTenancyServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenancy(
        this IServiceCollection services,
        Action<MultiTenancyOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new MultiTenancyOptions();
        configure?.Invoke(options);

        services.AddOptions<MultiTenancyOptions>();
        services.Configure(configure ?? (_ => { }));
        services.TryAddSingleton<ITenantAccessor, TenantAccessor>();

        if (options.EnableClaimResolver)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolver, ClaimTenantResolver>());
        }

        if (options.EnableHeaderResolver)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolver, HeaderTenantResolver>());
        }

        if (options.EnableQueryStringResolver)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolver, QueryStringTenantResolver>());
        }

        if (options.EnableHostResolver)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolver, HostTenantResolver>());
        }

        services.TryAddScoped<IgnoreTenantFilter>();

        return services;
    }
}

public static class MultiTenancyApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<TenantMiddleware>();
    }
}
