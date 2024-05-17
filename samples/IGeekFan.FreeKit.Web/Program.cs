using Autofac;
using Autofac.Extensions.DependencyInjection;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Modularity;
using System.Reflection;
using IGeekFan.FreeKit.Web;
using IGeekFan.FreeKit.Extras.Security;
using Autofac.Core;
using Microsoft.AspNetCore.DataProtection;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
    {
        //1.获取所有的程序集合，然后根据FullName，一般为项目名，过滤具体的程序集
        Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r =>
               r.FullName.Contains("IGeekFan.FreeKit.Web") 
               || r.FullName.Contains("Module1")
               || r.FullName.Contains("Module2")
            ).ToArray();
        containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));
        containerBuilder.RegisterModule(new UnitOfWorkModule(currentAssemblies));

        //2.根据程序集中的某个类获取程序集,直接使用params Assembly[]
        //Assembly[] currentAssemblies2 = new Assembly[] { typeof(Program).Assembly, typeof(Module1.Module1Startup).Assembly };
        //containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies2));

        ////3，通过params Type[]，内部解析Assembly。
        //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule), typeof(Module1.Module1Startup)));

    });
// Add services to the container.

IConfiguration c = builder.Configuration;

builder.Services
        .AddCustomMvc(c)
        .AddSwagger(c)
        .AddFreeSql(c)
        .AddModuleServices(c);

builder.Services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeyContext>();

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
app.UseAuthentication();

app.UseCurrentUserAccessor();

app.UseRouting()
    .UseEndpoints((endpoints) =>
    {
        endpoints.MapControllers();
    });

// Adds endpoints defined in modules
//var modules = app.Services.GetRequiredService<IEnumerable<ModuleInfo>>();
//foreach (var module in modules)
//{
//    app.Map($"/{module.RoutePrefix}", builder =>
//    {
//        module.Startup.Configure(app, app.Environment);
//    });
//}

app.Run();