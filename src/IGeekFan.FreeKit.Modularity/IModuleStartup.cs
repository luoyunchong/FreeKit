using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.FreeKit.Modularity;

public interface IModuleStartup
{
    void ConfigureServices(IServiceCollection services);
    void Configure(WebApplication app, IWebHostEnvironment env);
}
