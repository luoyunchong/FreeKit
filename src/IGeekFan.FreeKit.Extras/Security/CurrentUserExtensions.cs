using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// CurrentUser扩展
/// </summary>
public static class CurrentUserExtensions
{
    #region 获取UserId
    /// <summary>
    /// 获取泛型用户Id
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    public static T? FindUserId<T>(this ICurrentUser currentUser) where T : struct
    {
        if (currentUser.Id == null) return default;
        if (typeof(T) == typeof(string))
        {
            return (T)(object)currentUser.Id;
        }

        if (typeof(T) == typeof(Guid))
        {
            Guid? userGuid = currentUser.FindUserIdToGuid();
            if (userGuid == null) return default;
            return (T)(object)userGuid;
        }

        if (typeof(T) == typeof(long))
        {
            long? userLong = currentUser.FindUserIdToLong();
            if (userLong == null) return default;
            return (T)(object)userLong;
        }

        if (typeof(T) == typeof(int))
        {
            long? userInt = currentUser.FindUserIdToInt();
            if (userInt == null) return default;
            return (T)(object)userInt;
        }

        return (T)(object)currentUser.Id;
    }
    public static long? FindUserIdToLong(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;
        return long.Parse(currentUser.Id);
    }

    public static int? FindUserIdToInt(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;
        return int.Parse(currentUser.Id);
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
    #endregion
    
    /// <summary>
    ///  姓名
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    public static string? FindName(this ICurrentUser currentUser)
    {
        Claim? claim = currentUser.FindClaim(FreeKitClaimTypes.Name);
        return claim?.Value;
    }

    /// <summary>
    ///  手机号
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    public static string? FindPhoneNumber(this ICurrentUser currentUser)
    {
        Claim? claim = currentUser.FindClaim(FreeKitClaimTypes.PhoneNumber);
        return claim?.Value;
    }
}