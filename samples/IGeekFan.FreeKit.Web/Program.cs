using Autofac;
using Autofac.Extensions.DependencyInjection;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Modularity;
using System.Reflection;
using IGeekFan.FreeKit.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
    {
        //1.获取所有的程序集合，然后根据FullName，一般为项目名，过滤具体的程序集
        Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r =>
               r.FullName.Contains("IGeekFan.FreeKit.Extras") || r.FullName.Contains("Module1")
            ).ToArray();
        containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));
        containerBuilder.RegisterModule(new UnitOfWorkModule(currentAssemblies));

        ////2.根据程序集中的某个类获取程序集
        //Assembly[] currentAssemblies2 = new Assembly[] { typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly };
        //containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies2));

        ////3.直接使用params Assembly[] 的特性，直接作为FreeKitModule的参数
        //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly));

        ////4，通过params Type[]，内部解析Assembly。
        //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule), typeof(Module1.Module1Startup)));

    });
// Add services to the container.

IConfiguration c = builder.Configuration;

builder.Services
        .AddCustomMvc(c)
        .AddSwagger(c)
        .AddFreeSql(c)
        .AddModuleServices(c);


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
