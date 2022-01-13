namespace IGeekFan.FreeKit.Extras.AduitEntity;
public interface IUpdateAuditEntity<T>
{
    /// <summary>
    /// 最后修改人Id
    /// </summary>
    T UpdateUserId { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    string UpdateUserName { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    DateTime? UpdateTime { get; set; }
}

public interface IUpdateAuditEntity : IUpdateAuditEntity<long>
{

}
