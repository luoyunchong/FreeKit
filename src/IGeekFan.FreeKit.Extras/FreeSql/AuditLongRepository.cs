using FreeSql;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 审计仓储实现，用户主键类型为long、表的主键为Guid
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class AuditLongRepository<TEntity> : AuditDefaultRepository<TEntity, Guid, long>, IAuditBaseRepository<TEntity>
    where TEntity : class, new()
{
    public AuditLongRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager,
        currentUser)
    {
    }
}

/// <summary>
/// 审计仓储，用户主键类型为long
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class AuditTKeyLongRepository<TEntity, TKey> : AuditDefaultRepository<TEntity, TKey, long>
    where TEntity : class, new()
{
    public AuditTKeyLongRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(
        unitOfWorkManager,
        currentUser)
    {
    }
}