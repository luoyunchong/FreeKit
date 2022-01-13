using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AduitEntity;

//[Index("{tablename}_uk_id_index", "id asc", true)]
public abstract class Entity<T> : IEntity<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
    public T Id { get; set; }
}

[Serializable]
public abstract class Entity : Entity<long>
{
}

[Serializable]
public class FullAduitEntity : FullAduitEntity<long, long?>
{
}

public class FullAduitEntity<T, U> : Entity<T>, ICreateAduitEntity<U>, IUpdateAuditEntity<U>, IDeleteAduitEntity<U>
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
    public U UpdateUserId { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    [Column(Position = -3)]
    public string UpdateUserName { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(Position = -3)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 删除人id
    /// </summary>
    [Column(Position = -2)]
    public U DeleteUserId { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    [Column(Position = -2)]
    public string DeleteUserName { get; set; }

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

