using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AduitEntity;

[Serializable]
public class FullAduitEntity : FullAduitEntity<Guid, Guid>, IFullAduitEntity<Guid, Guid>
{
}

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="T">当前主键类型</typeparam>
/// <typeparam name="U">用户表主键类型</typeparam>
public class FullAduitEntity<T, U> : Entity<T>, ICreateAduitEntity<U>, IUpdateAuditEntity<U>, IDeleteAduitEntity<U>
    where T : struct
    where U : struct
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    [Column(Position = -4)]
    public U CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Position = -4)]
    public string CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Position = -4)]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 最后修改人Id
    /// </summary>
    [Column(Position = -3)]
    public U? UpdateUserId { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    [Column(Position = -3)]
    public string? UpdateUserName { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(Position = -3)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 删除人id
    /// </summary>
    [Column(Position = -2)]
    public U? DeleteUserId { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    [Column(Position = -2)]
    public string? DeleteUserName { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(Position = -2)]
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Column(Position = -1)]
    public bool IsDeleted { get; set; }
}

