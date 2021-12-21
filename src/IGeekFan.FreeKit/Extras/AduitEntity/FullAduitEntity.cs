using System;

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

public interface ICreateAduitEntity
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    long CreateUserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreateTime { get; set; }
}

public interface IUpdateAuditEntity
{
    /// <summary>
    /// 最后修改人Id
    /// </summary>
    long? UpdateUserId { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    DateTime? UpdateTime { get; set; }
}

public interface IDeleteAduitEntity : ISoftDelete
{
    /// <summary>
    /// 删除人id
    /// </summary>
    long? DeleteUserId { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    DateTime? DeleteTime { get; set; }
}



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
