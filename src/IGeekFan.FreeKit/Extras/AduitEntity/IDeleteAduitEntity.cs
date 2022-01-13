namespace IGeekFan.FreeKit.Extras.AduitEntity;

public interface IDeleteAduitEntity : IDeleteAduitEntity<long?>
{

}

public interface IDeleteAduitEntity<T> : ISoftDelete
{
    /// <summary>
    /// 删除人id
    /// </summary>
    T DeleteUserId { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    string DeleteUserName { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    DateTime? DeleteTime { get; set; }
}