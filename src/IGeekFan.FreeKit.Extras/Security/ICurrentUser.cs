using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Dependency;

namespace IGeekFan.FreeKit.Extras.Security;

public interface ICurrentUser : ICurrentUser<string>
{

}
public interface ICurrentUser<T> : ITransientDependency
{
    /// <summary>
    /// 是否登录
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    T? Id { get; }

    /// <summary>
    /// 登录名，用户名，唯一值
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 昵称
    /// </summary>
    string? NickName { get; }

    /// <summary>
    /// 邮件
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// 角色
    /// </summary>
    string[] Roles { get; }

    Claim FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();

    bool IsInRole(string roleId);
}

public static class ICurrentUserExtensions
{
    public static long? FindUserIdToLong(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;
        return long.Parse(currentUser.Id);
    }

    public static Guid? FindUserIdToGuid(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;

        if (Guid.TryParse(currentUser.Id, out Guid guid))
        {
            return guid;
        }
        return null;
    }

    public static string? FindRealName(this ICurrentUser currentUser)
    {
        Claim? claim = currentUser.FindClaim(ClaimTypes.Surname);
        return claim?.Value;
    }

    /// <summary>
    /// 无法确定用户的Id是什么类型，所以将方法复制到自己项目中，可直接修改此字段
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    //public static string? FindUserId(this ICurrentUser currentUser)
    //{
    //    return currentUser.Id;
    //}
}
