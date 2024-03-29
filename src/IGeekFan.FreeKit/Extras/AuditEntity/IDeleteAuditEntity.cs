﻿namespace IGeekFan.FreeKit.Extras.AuditEntity;

/// <summary>
/// 参考<see cref="IDeleteAuditEntity"/> 
/// </summary>
/// <typeparam name="T">用户表主键</typeparam>
public interface IDeleteAuditEntity<T> : ISoftDelete where T : struct, IEquatable<T>
{
    /// <summary>
    /// 删除人id
    /// </summary>
    T? DeleteUserId { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    string? DeleteUserName { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    DateTime? DeleteTime { get; set; }
}


/// <summary>
/// 使用此接口可存储删除信息（删除人id，删除人，删除时间）
/// </summary>
public interface IDeleteAuditEntity : IDeleteAuditEntity<Guid>
{

}
