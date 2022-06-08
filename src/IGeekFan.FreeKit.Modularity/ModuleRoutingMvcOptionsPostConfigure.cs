using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Modularity;

public class ModuleRoutingMvcOptionsPostConfigure : IPostConfigureOptions<MvcOptions>
{
    private readonly IEnumerable<ModuleInfo> _modules;

    public ModuleRoutingMvcOptionsPostConfigure(IEnumerable<ModuleInfo> modules)
    {
        _modules = modules;
    }

    public void PostConfigure(string name, MvcOptions options)
    {
        options.Conventions.Add(new ModuleRoutingConvention(_modules));
    }
}
