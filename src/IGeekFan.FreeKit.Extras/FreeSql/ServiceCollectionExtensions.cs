// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 复合主键,二个主键的仓储
/// </summary>
public static class ServiceCollectionExtensions
{
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
        else if(typeof(long) == typeUserkey)
        {
            services.TryAddScoped(typeof(IAuditBaseRepository<>), typeof(AuditLongRepository<>));
            services.TryAddScoped(typeof(IAuditBaseRepository<,>), typeof(AuditTKeyLongRepository<,>));
        }
        else if(typeof(int) == typeUserkey)
        {
            services.TryAddScoped(typeof(IAuditBaseRepository<>), typeof(AuditLongRepository<>));
            services.TryAddScoped(typeof(IAuditBaseRepository<,>), typeof(AuditTKeyLongRepository<,>));
        }
        else
        {
            throw new NotSupportedException("用户ID仅支持Guid/long/int类型");
        }
        return services;
    }
}