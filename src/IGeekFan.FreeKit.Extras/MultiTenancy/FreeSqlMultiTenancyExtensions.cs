using FreeSql;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public static class FreeSqlMultiTenancyExtensions
{
    private static readonly AsyncLocal<bool> _ignoreTenant = new();

    public static bool IsTenantFilterDisabled => _ignoreTenant.Value;

    public static IFreeSql UseMultiTenancyFilter(
        this IFreeSql fsql,
        Func<Guid?>? getCurrentTenantId = null)
    {
        getCurrentTenantId ??= () => TenantAccessor.CurrentTenant?.Id;

        fsql.GlobalFilter.Apply<ITenant>("MultiTenancy", 
            t => t.TenantId == getCurrentTenantId() || getCurrentTenantId() == null || IsTenantFilterDisabled);

        return fsql;
    }

    public static IFreeSql UseMultiTenancyFilter<T>(
        this IFreeSql<T> fsql,
        Func<Guid?>? getCurrentTenantId = null) where T : class
    {
        getCurrentTenantId ??= () => TenantAccessor.CurrentTenant?.Id;

        fsql.GlobalFilter.Apply<ITenant>("MultiTenancy", 
            t => t.TenantId == getCurrentTenantId() || getCurrentTenantId() == null || IsTenantFilterDisabled);

        return fsql;
    }

    public static ISelect<TEntity> IgnoreTenantFilter<TEntity>(this ISelect<TEntity> select) 
        where TEntity : class
    {
        return select.DisableGlobalFilter("MultiTenancy");
    }

    public static IDisposable BeginIgnoreTenant()
    {
        _ignoreTenant.Value = true;
        return new IgnoreTenantScope(() => _ignoreTenant.Value = false);
    }

    private class IgnoreTenantScope(Action? disposeAction) : IDisposable
    {
        public void Dispose() => disposeAction?.Invoke();
    }
}
