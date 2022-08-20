// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AuditEntity;

[Serializable]
public class FullAuditEntity : FullAuditEntity<Guid, Guid>, IFullAuditEntity<Guid, Guid>
{
}

/// <summary>
/// 包含审计实体基类,包含 创建、修改、删除加主键等10个字段，其实T为当前主键类型，U为用户表主键类型
/// </summary>
/// <typeparam name="T">当前主键类型</typeparam>
/// <typeparam name="U">用户表主键类型</typeparam>
public class FullAuditEntity<T, U> : Entity<T>, ICreateAuditEntity<U>, IUpdateAuditEntity<U>, IDeleteAuditEntity<U>
    where T :  IEquatable<T>
    where U : struct, IEquatable<U>
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    [Column(Position = -4)]
    public virtual U? CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Position = -4)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Position = -4)]
    public virtual DateTime CreateTime { get; set; }

    /// <summary>
    /// 最后修改人Id
    /// </summary>
    [Column(Position = -3)]
    public virtual U? UpdateUserId { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    [Column(Position = -3)]
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(Position = -3)]
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 删除人id
    /// </summary>
    [Column(Position = -2)]
    public virtual U? DeleteUserId { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    [Column(Position = -2)]
    public virtual string? DeleteUserName { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(Position = -2)]
    public virtual DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Column(Position = -1)]
    public virtual bool IsDeleted { get; set; }
}

