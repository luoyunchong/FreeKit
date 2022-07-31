// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.Security;

public class CurrentUser : CurrentUser<string>, ICurrentUser
{
    public CurrentUser(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }
}


/// <summary>
/// 当前用户上下文
/// </summary>
/// <typeparam name="T"></typeparam>
public class CurrentUser<T> : ICurrentUser<T> where T : IEquatable<T>
{
    protected readonly ClaimsPrincipal ClaimsPrincipal;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
    }

    public virtual bool IsAuthenticated => ClaimsPrincipal.FindUserId() != null;

    public virtual T? Id => ClaimsPrincipal.FindUserId<T>();

    public virtual string? UserName => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

    public virtual string? NickName => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

    public virtual string? Email => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    public virtual string[] Roles => FindClaims(ClaimTypes.Role).Select(c => c.Value.ToString()).ToArray();

    public virtual Claim? FindClaim(string claimType)
    {
        return ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return ClaimsPrincipal.Claims.Where(c => c.Type == claimType).ToArray();
    }

    public virtual Claim[] GetAllClaims()
    {
        return ClaimsPrincipal.Claims.ToArray();
    }

    public virtual bool IsInRole(string roleId)
    {
        return FindClaims(ClaimTypes.Role).Any(c => c.Value == roleId);
    }
}
