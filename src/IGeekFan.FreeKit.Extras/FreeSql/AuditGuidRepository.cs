using FreeSql;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 审计仓储，表的主键/用户主键类型都为Guid
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class AuditGuidRepository<TEntity> : AuditDefaultRepository<TEntity, Guid, Guid>, IAuditBaseRepository<TEntity>
    where TEntity : class, new()
{
    public AuditGuidRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager,
        currentUser)
    {
    }
}

/// <summary>
/// 审计仓储，用户主键类型为Guid
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class AuditTKeyGuidRepository<TEntity, TKey> : AuditDefaultRepository<TEntity, TKey, Guid>, IAuditBaseRepository<TEntity, TKey>
    where TEntity : class, new()
{
    public AuditTKeyGuidRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager,
        currentUser)
    {
    }
}
