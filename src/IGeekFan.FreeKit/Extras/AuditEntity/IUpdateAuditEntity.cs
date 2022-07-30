namespace IGeekFan.FreeKit.Extras.AuditEntity;

/// <summary>
/// 参考<see cref="IUpdateAuditEntity"/> 
/// </summary>
/// <typeparam name="T">用户表主键</typeparam>
public interface IUpdateAuditEntity<T> where T : struct, IEquatable<T>
{
    /// <summary>
    /// 最后修改人Id
    /// </summary>
    T? UpdateUserId { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    string? UpdateUserName { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    DateTime? UpdateTime { get; set; }
}

/// <summary>
///  使用此接口可存储最后更新信息（最后修改人Id，修改人，修改时间）
/// </summary>
public interface IUpdateAuditEntity : IUpdateAuditEntity<Guid>
{

}
