namespace IGeekFan.FreeKit.Extras.AduitEntity;
public interface IEntity<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    T Id { get; set; }
}

public interface IEntity : IEntity<long>
{
}
