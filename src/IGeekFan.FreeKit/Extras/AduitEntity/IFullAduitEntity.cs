namespace IGeekFan.FreeKit.Extras.AduitEntity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">当前表主键类型</typeparam>
/// <typeparam name="U">用户表id类型</typeparam>
public interface IFullAduitEntity<T, U> : ICreateAduitEntity<U>, IUpdateAuditEntity<U>, IDeleteAduitEntity<U>
    where T : struct
    where U : struct
{

}