namespace IGeekFan.FreeKit.Extras.AduitEntity;

public interface IEntityDto
{
}

public interface IEntityDto<TKey> : IEntityDto where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}

public abstract class EntityDto<TKey> : IEntityDto<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public TKey Id { get; set; }
}

public abstract class EntityDto : EntityDto<Guid>
{
}
