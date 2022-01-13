namespace IGeekFan.FreeKit.Extras.AduitEntity;

public interface IEntityDto
{
}

public interface IEntityDto<TKey> : IEntityDto
{
    TKey Id { get; set; }
}

public abstract class EntityDto<TKey> : IEntityDto<TKey>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public TKey Id { get; set; }
}

public abstract class EntityDto : EntityDto<long>
{
}
