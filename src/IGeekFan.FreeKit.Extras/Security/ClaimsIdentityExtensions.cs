// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IGeekFan.FreeKit.Extras.Extensions;
using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public static class ClaimsIdentityExtensions
{
    public static string? FindUserId(this ClaimsPrincipal principal)
    {
        Claim? userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        return userIdOrNull?.Value;
    }
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

    public static string? FindUserName(this ClaimsPrincipal principal)
    {
        Claim? userNameOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        return userNameOrNull?.Value;
    }
}

