// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Dependency;

namespace IGeekFan.FreeKit.Extras.Security;

public interface ICurrentUser : ICurrentUser<string>
{
}

public interface ICurrentUser<T> : ITransientDependency where T : IEquatable<T>
{
    /// <summary>
    /// 是否认证（即已登录）
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    T? Id { get; }

    /// <summary>
    /// 登录名，用户名，唯一值
    /// </summary>
    string? UserName { get; }

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

    Claim? FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();

    bool IsInRole(string roleId);
}