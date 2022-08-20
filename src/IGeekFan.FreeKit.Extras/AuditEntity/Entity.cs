// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AuditEntity;

public abstract class Entity : Entity<Guid>
{
}

//[Index("{tablename}_uk_id_index", "id asc", true)]
[Serializable]
public abstract class Entity<T> : IEntity<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
    public virtual T Id { get; set; }

    public virtual object[] GetKeys()
    {
        return new object[] { Id };
    }

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Id = {Id}";
    }
}


/// <summary>
/// 审计信息-新增
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
/// <typeparam name="TUKey">用户表类型</typeparam>
public class CreateAuditEntity<TKey, TUKey> : Entity<TKey>, ICreateAuditEntity<TUKey>
    where TKey : IEquatable<TKey>
    where TUKey : struct, IEquatable<TUKey>
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    public virtual TUKey? CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public virtual string? CreateUserName { get; set; }


    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime CreateTime { get; set; }
}

public class CreateAuditEntity : CreateAuditEntity<Guid, Guid>, ICreateAuditEntity
{

}
