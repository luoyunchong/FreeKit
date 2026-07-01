using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.FreeKit.Modularity;

/// <summary>
/// Extension methods for configuring registered modules.
/// </summary>
public static class ModuleApplicationBuilderExtensions
{
    /// <summary>
    /// Runs the Configure method of every registered module.
    /// </summary>
    public static WebApplication ConfigureModules(this WebApplication app)
    {
        var modules = app.Services.GetRequiredService<IEnumerable<ModuleInfo>>();
        foreach (var module in modules)
        {
            module.Startup.Configure(app, app.Environment);
        }

        return app;
    }
}
