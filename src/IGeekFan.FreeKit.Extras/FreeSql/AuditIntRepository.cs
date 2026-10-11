// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 审计仓储实现，用户主键类型为Int、表的主键为Guid
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class AuditIntRepository<TEntity> : AuditDefaultRepository<TEntity, Guid, int>, IAuditBaseRepository<TEntity>
    where TEntity : class, new()
{
    public AuditIntRepository(IUnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager,
        currentUser)
    {
    }
}

/// <summary>
/// 审计仓储，用户主键类型为Int
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class AuditTKeyIntRepository<TEntity, TKey> : AuditDefaultRepository<TEntity, TKey, long>
    where TEntity : class, new()
{
    public AuditTKeyIntRepository(IUnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(
        unitOfWorkManager,
        currentUser)
    {
    }
}
