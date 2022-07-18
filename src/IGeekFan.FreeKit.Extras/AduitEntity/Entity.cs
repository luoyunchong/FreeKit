using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AduitEntity;

namespace IGeekFan.FreeKit.Extras.AduitEntity;

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
    public T Id { get; set; }

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
public class CreateAduitEntity<TKey, TUKey> : Entity<TKey>, ICreateAduitEntity<TUKey>
    where TKey : IEquatable<TKey>
    where TUKey : struct, IEquatable<TUKey>
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    public TUKey? CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreateUserName { get; set; }


    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}

public class CreateAduitEntity : CreateAduitEntity<Guid, Guid>, ICreateAduitEntity
{

}
