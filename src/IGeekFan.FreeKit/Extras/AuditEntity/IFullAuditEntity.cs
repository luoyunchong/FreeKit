namespace IGeekFan.FreeKit.Extras.AuditEntity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey">当前表主键类型</typeparam>
/// <typeparam name="UKey">用户表id类型</typeparam>
public interface IFullAuditEntity<TKey, UKey> : ICreateAuditEntity<UKey>, IUpdateAuditEntity<UKey>, IDeleteAuditEntity<UKey>
    where TKey : IEquatable<TKey>
    where UKey : struct, IEquatable<UKey>
{

}