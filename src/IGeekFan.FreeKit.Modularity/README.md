
### 单体模块化

- https://github.com/thinktecture-labs/aspnetcore-modular-monolith

因为类名如Autofac和aspnetcore中的类相同，将类名修改成如下名称

- Module.cs-->ModuleInfo.cs
- IStartup.cs-->IModuleStartup.cs


创建一个类库名Module1，并引用 AspNetCore包
```xml
   <ItemGroup>
		<FrameworkReference Include="Microsoft.AspNetCore.App" />
   </ItemGroup>
```

集成接口，可在`ConfigureServices`中注入任何服务，在`Configure`方法中注入中间件
```csharp
public class Module1Startup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddSingleton<ITestService, TestService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
            endpoints.MapGet("/TestEndpoint",
                async context =>
                {
                    await context.Response.WriteAsync("Hello World from TestEndpoint in Module 1");
                }).RequireAuthorization()
        );
    }
}
```

使用控制器
```csharp
using Microsoft.AspNetCore.Mvc;

[Route("[module]/[controller]")]
public class TestController : Controller
{
    [HttpGet]
    public ActionResult<string> Index()
    {
        return "Hello World from TestController in Module 1";
    }

    /// <summary>
    /// InterModule
    /// </summary>
    /// <returns></returns>
    [HttpGet("InterModule")]
    public ActionResult<string> InterModule()
    {
        return $"{0} in TestController in Module 1";
    }
}
```


- 主程序`Program`

```csharp
// Register a convention allowing to us to prefix routes to modules.
builder.Services.AddTransient<IPostConfigureOptions<MvcOptions>, ModuleRoutingMvcOptionsPostConfigure>();

// Adds module1 with the route prefix module-1
builder.Services.AddModule<Module1.Module1Startup>("module-1");

// Adds module2 with the route prefix module-2
builder.Services.AddModule<Module2.Module2Startup>("module-2");
```


- 注入中间件
```csharp
// Adds endpoints defined in modules
var modules = app.Services.GetRequiredService<IEnumerable<ModuleInfo>>();
foreach (var module in modules)
{
    app.Map($"/{module.RoutePrefix}", builder =>
    {
        builder.UseRouting();
        module.Startup.Configure(builder, app.Environment);
    });
}

```