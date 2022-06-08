using Autofac;
using Autofac.Extensions.DependencyInjection;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.SnakeCaseQuery;
using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
    {
        //1.获取所有的程序集合，然后根据FullName，一般为项目名，过滤具体的程序集
        Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("IGeekFan.FreeKit.Extras") || r.FullName.Contains("Module1")).ToArray();
        containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));

        ////2.根据程序集中的某个类获取程序集
        //Assembly[] currentAssemblies2 = new Assembly[] { typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly };
        //containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies2));

        ////3.直接使用params Assembly[] 的特性，直接作为FreeKitModule的参数
        //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly));

        ////4，通过params Type[]，内部解析Assembly。
        //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule), typeof(Module1.Module1Startup)));

    });

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ValueProviderFactories.Add(new CamelCaseValueProviderFactory());
});
builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, CamelCaseApiDescriptionProvider>());

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IGeekFan.FreeKit.Web", Version = "v1" });
});


IFreeSql fsql = new FreeSql.FreeSqlBuilder()
    .UseConnectionString(FreeSql.DataType.Sqlite, builder.Configuration["ConnectionStrings:DefaultConnection"])
    .UseAutoSyncStructure(true)
    .UseMonitorCommand(
        cmd => Trace.WriteLine("\r\n线程" + Thread.CurrentThread.ManagedThreadId + ": " + cmd.CommandText)
  ).Build();
builder.Services.AddSingleton(fsql);
// Register a convention allowing to us to prefix routes to modules.
builder.Services.AddTransient<IPostConfigureOptions<MvcOptions>, ModuleRoutingMvcOptionsPostConfigure>();

// Adds module1 with the route prefix module-1
builder.Services.AddModule<Module1.Module1Startup>("module-1");

// Adds module2 with the route prefix module-2
builder.Services.AddModule<Module2.Module2Startup>("module-2");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGeekFan.FreeKit.Web v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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

app.Run();
