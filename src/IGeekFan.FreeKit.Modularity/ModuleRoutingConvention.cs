using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace IGeekFan.FreeKit.Modularity;

/// <summary>
/// Adds the route prefix to all actions 
/// </summary>
public class ModuleRoutingConvention : IActionModelConvention
{
    private readonly IEnumerable<ModuleInfo> _modules;

    public ModuleRoutingConvention(IEnumerable<ModuleInfo> modules)
    {
        _modules = modules;
    }

    public void Apply(ActionModel action)
    {
        var module = _modules.FirstOrDefault(m => m.Assembly == action.Controller.ControllerType.Assembly);
        if (module == null)
        {
            return;
        }

        action.RouteValues.Add("module", module.RoutePrefix);
    }
}
