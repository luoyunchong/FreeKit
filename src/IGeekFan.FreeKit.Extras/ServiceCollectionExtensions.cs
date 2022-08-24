using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddHttpContextAccessor();

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
}