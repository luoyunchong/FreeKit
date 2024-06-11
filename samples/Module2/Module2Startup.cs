using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Module2;

public class Module2Startup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection servicess,IConfiguration c)
    {
        //services.AddSingleton<ITestService, TestService>();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
       
    }
}
