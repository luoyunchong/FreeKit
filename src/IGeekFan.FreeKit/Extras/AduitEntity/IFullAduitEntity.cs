namespace IGeekFan.FreeKit.Extras.AduitEntity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey">当前表主键类型</typeparam>
/// <typeparam name="UKey">用户表id类型</typeparam>
public interface IFullAduitEntity<TKey, UKey> : ICreateAduitEntity<UKey>, IUpdateAuditEntity<UKey>, IDeleteAduitEntity<UKey>
    where TKey : IEquatable<TKey>
    where UKey : struct, IEquatable<UKey>
{

}