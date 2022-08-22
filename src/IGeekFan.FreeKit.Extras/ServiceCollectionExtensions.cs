using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace IGeekFan.FreeKit.Extras;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 统一配置服务，简化内部多个配置细节
    /// </summary>
    /// <param name="services"></param>
    /// <param name="typeUserkey">用户表类型，默认为guid</param>
    /// <returns></returns>
    public static IServiceCollection AddFreeKitCore(this IServiceCollection services, Type? typeUserkey = null)
    {
        //(controller unitofwork fitler
        //当前登录人信息
        //当前登录人上下文的accessor
        //审计仓储
        //复合主键仓储)
        services
            .AddTransient<UnitOfWorkActionFilter>()
            .AddCurrentUser()
            .AddCurrentUserAccessor()
            .AddAuditRepostiory(typeUserkey)
            .AddCompositeRepostiory();

        services.AddDefaultRepository();
        services.TryAddScoped<UnitOfWorkManager>();

        return services;
    }


    /// <summary>
    /// 获取一下Scope Service 以执行 DbContext中的OnModelCreating
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void RunScopeService<T>(this IServiceProvider serviceProvider) where T : DbContext
    {
        using var serviceScope = serviceProvider.CreateScope();
        try
        {
            using var myDependency = serviceScope.ServiceProvider.GetRequiredService<T>();
        }
        catch (Exception ex)
        {
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ICurrentUser>>();
            logger.LogError(ex, "An error occurred.");
        }
    }
}