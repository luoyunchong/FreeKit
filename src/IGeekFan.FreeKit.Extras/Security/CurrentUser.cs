// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 默认的string 当前用户
/// </summary>
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
    /// <summary>
    /// 证件持有者
    /// </summary>
    protected readonly ClaimsPrincipal ClaimsPrincipal;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal = (httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal) ?? new  ClaimsPrincipal();
    }

    /// <inheritdoc />
    public virtual bool IsAuthenticated => ClaimsPrincipal.FindUserId() != null;

    /// <inheritdoc/>

    public virtual T? Id => ClaimsPrincipal.FindUserId<T>();

    /// <inheritdoc />
    public virtual string? UserName => ClaimsPrincipal.FindUserName();

    /// <inheritdoc />
    public virtual string? Email => ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == FreeKitClaimTypes.Email)?.Value;

    /// <inheritdoc />
    public virtual string[] Roles => FindClaims(FreeKitClaimTypes.Role).Select(c => c.Value.ToString()).ToArray();

    /// <inheritdoc />
    public virtual Guid? TenantId => ClaimsPrincipal.FindTenantId();

    /// <inheritdoc />
    public virtual string? TenantName => ClaimsPrincipal?.Claims?.FirstOrDefault(c => c.Type == FreeKitClaimTypes.TenantName)?.Value;

    /// <inheritdoc />
    public virtual Claim? FindClaim(string claimType)
    {
        return ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    /// <inheritdoc />
    public virtual Claim[] FindClaims(string claimType)
    {
        return ClaimsPrincipal.Claims.Where(c => c.Type == claimType).ToArray();
    }

    /// <inheritdoc />
    public virtual Claim[] GetAllClaims()
    {
        return ClaimsPrincipal.Claims.ToArray();
    }

    /// <inheritdoc />
    public virtual bool IsInRole(string roleId)
    {
        return FindClaims(FreeKitClaimTypes.Role).Any(c => c.Value == roleId);
    }
}