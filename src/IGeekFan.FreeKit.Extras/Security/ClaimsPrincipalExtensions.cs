// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IGeekFan.FreeKit.Extras.Extensions;
using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// ClaimsPrincipal 扩展方法
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string? FindUserId(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        return userIdOrNull?.Value;
    }

    #region 无法确定用户Id类型，默认转换支持Guid,Long,Int
    public static Guid? FindUserIdToGuid(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (Guid.TryParse(userIdOrNull.Value, out Guid guid))
        {
            return guid;
        }
        return null;
    }

    public static long? FindUserIdToLong(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (long.TryParse(userIdOrNull.Value, out long userid))
        {
            return userid;
        }
        return null;
    }

    public static int? FindUserIdToInt(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (int.TryParse(userIdOrNull.Value, out int userid))
        {
            return userid;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentUser"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FindUserId<T>(this ClaimsPrincipal currentUser) where T : IEquatable<T>
    {
        string? userid = currentUser.FindUserId();
        if (userid == null) return default;
        if (typeof(T) == typeof(string))
        {
            return (T)(object)userid;
        }

        if (typeof(T) == typeof(Guid))
        {
            return (T)(object)currentUser.FindUserIdToGuid()!;
        }

        if (typeof(T) == typeof(long))
        {
            return (T)(object)currentUser.FindUserIdToLong()!;
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)currentUser.FindUserIdToInt()!;
        }

        return (T)(object)currentUser.FindUserId()!;
    }
    #endregion

}

