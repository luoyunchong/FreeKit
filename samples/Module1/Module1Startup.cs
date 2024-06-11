using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module1.Domain;

namespace Module1;

public class Module1Startup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection services,IConfiguration c)
    {
        //services.AddSingleton<ITestService, TestService>();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        var freeSql = app.Services.GetRequiredService<IFreeSql>();
        freeSql.CodeFirst.SyncStructure(ReflexHelper.GetTypesByTableAttribute(typeof(Song)));
    }
}
