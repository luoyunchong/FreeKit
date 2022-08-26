using System.Diagnostics;
using Autofac;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using FreeSql;
using IGeekFan.FreeKit.Email;
using IGeekFan.FreeKit.Extras.FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Security.Claims;
using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.xUnit
{
    public class Startup
    {
        IConfiguration configuration;

        // 自定义 host 构建
        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new (FreeKitClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
                new (FreeKitClaimTypes.UserName,"igeekfan"),
                new (FreeKitClaimTypes.Name,"天上月"),
                new (FreeKitClaimTypes.Email,"igeekfan@foxmail.com")
            }));
            Thread.CurrentPrincipal = claimsPrincipal;
            hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((context, builder) =>
                {
                    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();
                    // 注册配置
                    builder.AddJsonFile("appsettings.json");
                })
                .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
                {
                    //1.获取所有的程序集合，然后根据FullName，一般为项目名，过滤具体的程序集
                    Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r =>
                        r.FullName.Contains("IGeekFan.FreeKit.xUnit")
                        || r.FullName.Contains("IGeekFan.FreeKit.Extras")
                    ).ToArray();
                    containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));

                    ////2.根据程序集中的某个类获取程序集
                    //Assembly[] currentAssemblies2 = new Assembly[] { typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly };
                    //containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies2));

                    ////3.直接使用params Assembly[] 的特性，直接作为FreeKitModule的参数
                    //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule).Assembly, typeof(Module1.Module1Startup).Assembly));

                    ////4，通过params Type[]，内部解析Assembly。
                    //containerBuilder.RegisterModule(new FreeKitModule(typeof(FreeKitModule), typeof(Module1.Module1Startup)));
                })
                .ConfigureServices((context, services) =>
                {
                    configuration = context.Configuration;
                    // 注册自定义服务
                    services.AddEmailSender(configuration);
                    // services.AddEmailSender(r =>
                    // {
                    //     r.Host = "smtp.163.com";
                    //     r.Port = 25;
                    //     r.EnableSsl = true;
                    //     r.UserName = "igeekfan@163.com";
                    //     r.Password = "";
                    //     r.Domain = "";
                    // });
                }).UseSerilog();
        }

        // 支持的形式：
        // ConfigureServices(IServiceCollection services)
        // ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
        // ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            #region fsql

            Func<IServiceProvider, IFreeSql> fsql = r =>
            {
                IFreeSql fsql = new FreeSqlBuilder()
                        .UseConnectionString(DataType.Sqlite, configuration["ConnectionStrings:Sqlite"])
                        .UseAutoSyncStructure(true)
                        .UseNoneCommandParameter(true)
                        .UseGenerateCommandParameterWithLambda(false)
                        .UseLazyLoading(false)
                        .UseMonitorCommand(
                            cmd => Trace.WriteLine("\r\n线程" + Thread.CurrentThread.ManagedThreadId + ": " + cmd.CommandText)
                        )
                        .Build();
                fsql.GlobalFilter.Apply<ISoftDelete>("IsDeleted", a => a.IsDeleted == false);
                return fsql;
            };

            services.AddSingleton(fsql);
            services.AddFreeRepository();
            services.AddScoped<UnitOfWorkManager>();
            services.AddCompositeRepostiory();
            services.AddAuditRepostiory().AddCurrentUser();
            #endregion

            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddHttpClient();
            services.AddHttpContextAccessor();
        }

        // 可以添加要用到的方法参数，会自动从注册的服务中获取服务实例，类似于 asp.net core 里 Configure 方法
        public void Configure(IServiceProvider applicationServices)
        {
            // 有一些测试数据要初始化可以放在这里
            // InitData();
        }
    }
}