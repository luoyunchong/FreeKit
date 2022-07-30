namespace IGeekFan.FreeKit.Extras.AuditEntity;

/// <summary>
/// 参考<see cref="ICreateAuditEntity"/> 
/// </summary>
/// <typeparam name="T">用户表主键</typeparam>
public interface ICreateAuditEntity<T> where T : struct, IEquatable<T>
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    T? CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    string? CreateUserName { get; set; }


    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreateTime { get; set; }
}

/// <summary>
/// 使用此接口可存储创建者信息（创建者id，创建人，创建时间）
/// </summary>
public interface ICreateAuditEntity : ICreateAuditEntity<Guid>
{

}