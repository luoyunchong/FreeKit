using System.Linq;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace IGeekFan.FreeKit.Web.Controllers;

/// <summary>
/// Test
/// </summary>
[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    public TestController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    [HttpGet("GetCurrentAssemblyService")]
    public (int, IEnumerable<IComponentRegistration>) GetCurrentAssemblyService()
    {
        var assemblyRegistry = _serviceProvider.GetAutofacRoot().ComponentRegistry.Registrations.Where(r =>
            r.Services.Any(u => u.Description.Contains("IGeekFan"))
            || r.Services.Any(u => u.Description.Contains("Module1"))
            || r.Services.Any(u => u.Description.Contains("Module2"))
        ).ToList();

        return (assemblyRegistry.Count,assemblyRegistry);
    }
    [HttpGet("GetAllService")]
    public IEnumerable<IComponentRegistration> GetAllService()
    {
        List<string> allist = new List<string>() { "IGeekFan", "Module1", "Module2" };
        return _serviceProvider.GetAutofacRoot().ComponentRegistry.Registrations
            .Where(r => r.Services.Any(u => allist.Contains(u.Description))).ToList();
    }
}