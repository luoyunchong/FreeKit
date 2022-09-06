// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 审计仓储，表的主键为Guid
/// </summary>
/// <typeparam name="TEntity">实体</typeparam>
public interface IAuditBaseRepository<TEntity> : IBaseRepository<TEntity, Guid> where TEntity : class
{
}

/// <summary>
/// 审计仓储
/// </summary>
/// <typeparam name="TEntity">实体</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IAuditBaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
{
}