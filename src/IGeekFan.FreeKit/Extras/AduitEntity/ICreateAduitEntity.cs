namespace IGeekFan.FreeKit.Extras.AduitEntity;

public interface ICreateAduitEntity<T>
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
public interface ICreateAduitEntity : ICreateAduitEntity<long>
{

}