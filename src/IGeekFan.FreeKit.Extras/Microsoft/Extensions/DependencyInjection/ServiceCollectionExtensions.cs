using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
 
    public static IServiceCollection AddUnitOfWorkManager<T>(this IServiceCollection serviceCollection) where T : class
    {
        serviceCollection.AddTransient<UnitOfWorkActionFilter>();
        serviceCollection.TryAddScoped<UnitOfWorkManager<T>>();
        return serviceCollection;
    }

    public static IServiceCollection AddUnitOfWorkManager(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<UnitOfWorkActionFilter>();
        serviceCollection.TryAddScoped<UnitOfWorkManager>();

        return serviceCollection;
    }

    /// <summary>
    /// 统一配置服务，简化内部多个配置细节
    /// </summary>
    public static IServiceCollection AddFreeKitCore(this IServiceCollection services, Type? typeUserkey = null)
    {
        //(controller unitofwork fitler
        //当前登录人信息
        //当前登录人上下文的accessor
        //审计仓储
        //复合主键仓储)
        services.AddHttpContextAccessor();
        services.AddCurrentUser()
            .AddCurrentUserAccessor()
            .AddAuditRepostiory(typeUserkey)
            .AddCompositeRepostiory();

        services.AddDefaultRepository();

        return services;
    }


    /// <summary>
    /// 批量注入复合主键的 Repository
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCompositeRepostiory(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IBaseRepository<,,>), typeof(DefaultRepository<,,>));
        services.TryAddScoped(typeof(BaseRepository<,,>), typeof(DefaultRepository<,,>));
        return services;
    }

    /// <summary>
    /// 用户Id是Guid或long类型的审计仓储批量注入,默认注入用户主键为Guid类型
    /// </summary>
    /// <param name="services"></param>
    /// <param name="typeUserkey">支持typeof(Guid)/typeof(long)指定用户的主键类型，方便绑定 CreateUserId\UpdateUserId等字段</param>
    /// <returns></returns>
    public static IServiceCollection AddAuditRepostiory(this IServiceCollection services, Type? typeUserkey = null)
    {
        if (typeUserkey == null || typeof(Guid) == typeUserkey)
        {
            services.TryAddScoped(typeof(IAuditBaseRepository<>), typeof(AuditGuidRepository<>));
            services.TryAddScoped(typeof(IAuditBaseRepository<,>), typeof(AuditTKeyGuidRepository<,>));
        }
        else if (typeof(long) == typeUserkey)
        {
            services.TryAddScoped(typeof(IAuditBaseRepository<>), typeof(AuditLongRepository<>));
            services.TryAddScoped(typeof(IAuditBaseRepository<,>), typeof(AuditTKeyLongRepository<,>));
        }
        else if (typeof(int) == typeUserkey)
        {
            services.TryAddScoped(typeof(IAuditBaseRepository<>), typeof(AuditIntRepository<>));
            services.TryAddScoped(typeof(IAuditBaseRepository<,>), typeof(AuditTKeyIntRepository<,>));
        }
        else
        {
            throw new NotSupportedException("用户ID仅支持Guid/long/int类型");
        }

        return services;
    }


    /// <summary>
    /// 批量注入 IBaseRepository
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDefaultRepository(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IBaseRepository<>), typeof(GuidRepository<>));
        services.TryAddScoped(typeof(IBaseRepository<,>), typeof(DefaultRepository<,>));
        return services;
    }

    /// <summary>
    /// Adds a default implementation for the <see cref="ICurrentUser"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddTransient<ICurrentUser, CurrentUser>();
        services.TryAddTransient(typeof(ICurrentUser<>), typeof(CurrentUser<>));
        return services;
    }

    /// <summary>
    /// Adds a default implementation for the <see cref="ICurrentUserAccessor"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCurrentUserAccessor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<ICurrentUserAccessor, CurrentUserAccessor>();
        return services;
    }

    /// <summary>
    /// 请求过程中给单例的<see cref="ICurrentUserAccessor"/> 对象的CurrentUser赋值，方便全局获取用户信息
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCurrentUserAccessor(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CurrentUserAccessorMiddleware>();
    }
}