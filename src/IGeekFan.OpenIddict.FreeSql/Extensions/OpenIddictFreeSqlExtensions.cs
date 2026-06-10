using IGeekFan.OpenIddict.FreeSql.Entities;
using IGeekFan.OpenIddict.FreeSql.Stores;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Core;

namespace IGeekFan.OpenIddict.FreeSql;

/// <summary>
/// OpenIddict FreeSql 存储扩展方法
/// </summary>
public static class OpenIddictFreeSqlExtensions
{
    /// <summary>
    /// 注册 OpenIddict 的 FreeSql Store 实现（4 个 Store）
    /// </summary>
    public static OpenIddictCoreBuilder AddFreeSqlStores(this OpenIddictCoreBuilder builder)
    {
        builder.SetDefaultApplicationEntity<OpenIddictApplication>();
        builder.SetDefaultAuthorizationEntity<OpenIddictAuthorization>();
        builder.SetDefaultScopeEntity<OpenIddictScope>();
        builder.SetDefaultTokenEntity<OpenIddictToken>();

        builder.ReplaceApplicationStore<OpenIddictApplication, FreeSqlOpenIddictApplicationStore>(ServiceLifetime.Scoped);
        builder.ReplaceAuthorizationStore<OpenIddictAuthorization, FreeSqlOpenIddictAuthorizationStore>(ServiceLifetime.Scoped);
        builder.ReplaceScopeStore<OpenIddictScope, FreeSqlOpenIddictScopeStore>(ServiceLifetime.Scoped);
        builder.ReplaceTokenStore<OpenIddictToken, FreeSqlOpenIddictTokenStore>(ServiceLifetime.Scoped);

        return builder;
    }

    /// <summary>
    /// 同步 OpenIddict 表结构（CodeFirst）
    /// </summary>
    public static IFreeSql SyncOpenIddictTables(this IFreeSql freeSql)
    {
        freeSql.CodeFirst.SyncStructure(
            typeof(OpenIddictApplication),
            typeof(OpenIddictAuthorization),
            typeof(OpenIddictScope),
            typeof(OpenIddictToken));
        return freeSql;
    }
}
