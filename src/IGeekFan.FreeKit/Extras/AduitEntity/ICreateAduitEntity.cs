namespace IGeekFan.FreeKit.Extras.AduitEntity;

/// <summary>
/// 参考<see cref="ICreateAduitEntity"/> 
/// </summary>
/// <typeparam name="T">用户表主键</typeparam>
public interface ICreateAduitEntity<T> where T : struct
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    T CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    string CreateUserName { get; set; }


    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreateTime { get; set; }
}

/// <summary>
/// 使用此接口可存储创建者信息（创建者id，创建人，创建时间）
/// </summary>
public interface ICreateAduitEntity : ICreateAduitEntity<Guid>
{

}