using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Modularity;

public class ModularityApiTests
{
    [Fact]
    public void AddModuleRoutingRegistersMvcPostConfigureOnce()
    {
        ServiceCollection services = new();

        services.AddModuleRouting();
        services.AddModuleRouting();

        var registrations = services
            .Where(d => d.ServiceType == typeof(IPostConfigureOptions<MvcOptions>)
                        && d.ImplementationType == typeof(ModuleRoutingMvcOptionsPostConfigure))
            .ToList();

        Assert.Single(registrations);
    }

    [Fact]
    public void AddModuleWithStartupTypeRegistersModuleAndRunsConfigureServices()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        services.AddModuleRouting();
        services.AddModule(typeof(SampleModuleStartup), "sample", configuration);

        using ServiceProvider provider = services.BuildServiceProvider();
        ModuleInfo module = Assert.Single(provider.GetServices<ModuleInfo>());

        Assert.Equal("sample", module.RoutePrefix);
        Assert.IsType<SampleModuleStartup>(module.Startup);
        Assert.Same(typeof(SampleModuleStartup).Assembly, module.Assembly);
        Assert.IsType<SampleModuleService>(provider.GetRequiredService<ISampleModuleService>());
    }

    [Fact]
    public void ConfigureModulesRunsEachRegisteredModuleConfigure()
    {
        RuntimeModuleStartup.ConfigureCallCount = 0;

        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddModuleRouting();
        builder.Services.AddModule<RuntimeModuleStartup>("runtime", builder.Configuration);

        WebApplication app = builder.Build();

        app.ConfigureModules();

        Assert.Equal(1, RuntimeModuleStartup.ConfigureCallCount);
    }

    private interface ISampleModuleService
    {
    }

    private class SampleModuleService : ISampleModuleService
    {
    }

    public class SampleModuleStartup : IModuleStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration c)
        {
            services.AddScoped<ISampleModuleService, SampleModuleService>();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
        }
    }

    public class RuntimeModuleStartup : IModuleStartup
    {
        public static int ConfigureCallCount { get; set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration c)
        {
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            ConfigureCallCount++;
        }
    }
}
