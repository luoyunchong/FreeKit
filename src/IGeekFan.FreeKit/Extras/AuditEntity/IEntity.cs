namespace IGeekFan.FreeKit.Extras.AuditEntity;
public interface IEntity<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    T Id { get; set; }

    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object[] GetKeys();
}

public interface IEntity : IEntity<Guid>
{
}
