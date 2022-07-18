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

    /// <summary>
    /// 用户Id
    /// </summary>
    public override string? Id
    {
        get
        {
            return ClaimsPrincipal.FindUserId();
        }
    }
}


/// <summary>
/// 当前用户上下文
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CurrentUser<T> : ICurrentUser<T>
{
    private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();
    protected readonly ClaimsPrincipal ClaimsPrincipal;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
    }

    public bool IsAuthenticated => ClaimsPrincipal.FindUserId() != null ? true : false;

    public virtual T? Id => throw new Exception("需要重写");

    public string UserName => ClaimsPrincipal.FindUserName();

    public string? NickName => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

    public string? Email => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    public string[] Roles => FindClaims(ClaimTypes.Role).Select(c => c.Value.ToString()).ToArray();

    public virtual Claim? FindClaim(string claimType)
    {
        return ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return ClaimsPrincipal.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return ClaimsPrincipal.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleId)
    {
        return FindClaims(ClaimTypes.Role).Any(c => c.Value == roleId);
    }
}
