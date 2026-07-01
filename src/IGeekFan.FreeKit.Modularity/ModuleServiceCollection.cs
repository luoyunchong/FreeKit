using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Modularity;

/// <summary>
/// Extension methods for registering modules.
/// </summary>
public static class ModuleServiceCollection
{
    /// <summary>
    /// Adds the MVC convention that replaces the [module] route value with the registered module route prefix.
    /// </summary>
    public static IServiceCollection AddModuleRouting(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<MvcOptions>, ModuleRoutingMvcOptionsPostConfigure>());
        return services;
    }

    /// <summary>
    /// Adds a module.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="routePrefix">The prefix of the routes to the module.</param>
    /// <param name="c"></param>
    /// <typeparam name="TStartup">The type of the startup class of the module.</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddModule<TStartup>(this IServiceCollection services, string routePrefix, IConfiguration c)
        where TStartup : IModuleStartup, new()
    {
        return services.AddModule(typeof(TStartup), routePrefix, c);
    }

    /// <summary>
    /// Adds a module by startup type.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="startupType">The startup type of the module.</param>
    /// <param name="routePrefix">The prefix of the routes to the module.</param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IServiceCollection AddModule(this IServiceCollection services, Type startupType, string routePrefix, IConfiguration c)
    {
        if (!typeof(IModuleStartup).IsAssignableFrom(startupType))
        {
            throw new ArgumentException($"{startupType.FullName} must implement {nameof(IModuleStartup)}.", nameof(startupType));
        }

        services.AddModuleRouting();

        // Register assembly in MVC so it can find controllers of the module.
        services.AddControllers().ConfigureApplicationPartManager(manager =>
        {
            var assembly = startupType.Assembly;
            bool exists = manager.ApplicationParts
                .OfType<AssemblyPart>()
                .Any(part => part.Assembly == assembly);

            if (!exists)
            {
                manager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
        });

        var startup = (IModuleStartup?)Activator.CreateInstance(startupType)
            ?? throw new InvalidOperationException($"Cannot create module startup instance: {startupType.FullName}.");
        startup.ConfigureServices(services, c);

        services.AddSingleton(new ModuleInfo(routePrefix, startup));

        return services;
    }
}
