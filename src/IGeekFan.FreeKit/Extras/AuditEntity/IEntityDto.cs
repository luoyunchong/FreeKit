﻿using System.ComponentModel.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.AuditEntity;

public interface IEntityDto
{
}

public interface IEntityDto<TKey> : IEntityDto where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    TKey Id { get; set; }
}

public abstract class EntityDto<TKey> : IEntityDto<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required]
    public TKey Id { get; set; }
}

public abstract class EntityDto : EntityDto<Guid>
{
}
