using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AduitEntity;

[Serializable]
public abstract class Entity : Entity<long>
{
}

//[Index("{tablename}_uk_id_index", "id asc", true)]
public abstract class Entity<T> : IEntity<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
    public T Id { get; set; }
}

