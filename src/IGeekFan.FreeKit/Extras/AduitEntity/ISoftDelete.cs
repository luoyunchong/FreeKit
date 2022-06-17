namespace IGeekFan.FreeKit.Extras.AduitEntity;

/// <summary>
///  软删除标志
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// 是否删除
    /// </summary>
    bool IsDeleted { get; set; }
}