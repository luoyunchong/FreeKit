using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Modularity;

/// <summary>
/// Contains information about a module
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// Gets the route prefix to all controller and endpoints in the module.
    /// </summary>
    public string RoutePrefix { get; }

    /// <summary>
    /// Gets the startup class of the module.
    /// </summary>
    public IModuleStartup Startup { get; }

    /// <summary>
    /// Gets the assembly of the module.
    /// </summary>
    public Assembly Assembly => Startup.GetType().Assembly;

    public ModuleInfo(string routePrefix, IModuleStartup startup)
    {
        RoutePrefix = routePrefix;
        Startup = startup;
    }
}
