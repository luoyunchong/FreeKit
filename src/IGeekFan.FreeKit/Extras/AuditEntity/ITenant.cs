namespace IGeekFan.FreeKit.Extras.AuditEntity;

/// <summary>
/// 多租户
/// </summary>
public interface ITenant 
{
    /// <summary>
    /// 租户的Id
    /// </summary>
    Guid? TenantId { get; set; }
}