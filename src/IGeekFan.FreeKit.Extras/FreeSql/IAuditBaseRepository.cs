// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public interface IAuditBaseRepository<TEntity> : IBaseRepository<TEntity, Guid> where TEntity : class
{
}

public interface IAuditBaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
{
}