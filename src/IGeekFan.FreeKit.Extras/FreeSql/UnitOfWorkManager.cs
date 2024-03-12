using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 支持泛型仓储的工作单元管理器
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnitOfWorkManager<T> : UnitOfWorkManager where T : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fsql"></param>
    public UnitOfWorkManager(IFreeSql<T> fsql) : base(fsql)
    {
    }
}