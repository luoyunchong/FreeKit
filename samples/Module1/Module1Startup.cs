using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Module1;

public class Module1Startup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddSingleton<ITestService, TestService>();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
      
    }
}
