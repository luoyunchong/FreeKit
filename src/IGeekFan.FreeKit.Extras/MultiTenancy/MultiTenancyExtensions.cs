using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public static class MultiTenancyServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenancy(
        this IServiceCollection services,
        Action<MultiTenancyOptions>? configure = null)
    {
        var options = new MultiTenancyOptions();
        configure?.Invoke(options);

        services.Configure(configure ?? (_ => { }));
        services.AddSingleton<ITenantAccessor, TenantAccessor>();

        if (options.EnableClaimResolver)
        {
            services.AddTransient<ITenantResolver, ClaimTenantResolver>();
        }

        if (options.EnableHeaderResolver)
        {
            services.AddTransient<ITenantResolver>(sp => 
                new HeaderTenantResolver(options.HeaderName));
        }

        if (options.EnableQueryStringResolver)
        {
            services.AddTransient<ITenantResolver>(sp => 
                new QueryStringTenantResolver(options.QueryStringKey));
        }

        if (options.EnableHostResolver)
        {
            services.AddTransient<ITenantResolver>(sp => 
                new HostTenantResolver(options.TenantDomainSuffix, options.UseSubdomain));
        }

        services.AddScoped<IgnoreTenantFilter>();

        return services;
    }
}

public static class MultiTenancyApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
