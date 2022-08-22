// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Dependency;

namespace IGeekFan.FreeKit.Extras.Security;

public interface ICurrentUser : ICurrentUser<string>
{
}

/// <summary>
/// 登录人信息上下文 
/// </summary>
/// <typeparam name="T"></typeparam>
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
    /// 邮件
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// 角色
    /// </summary>
    string[] Roles { get; }

    /// <summary>
    /// 租户Id
    /// </summary>
    Guid? TenantId { get; }
    
    /// <summary>
    /// 租户名
    /// </summary>
    string? TenantName { get; }

    /// <summary>
    /// 根据声明类别获取声明
    /// </summary>
    /// <param name="claimType"></param>
    /// <returns></returns>
    Claim? FindClaim(string claimType);

    /// <summary>
    /// 根据声明类别获取所有声明
    /// </summary>
    /// <param name="claimType"></param>
    /// <returns></returns>
    Claim[] FindClaims(string claimType);

    /// <summary>
    /// 获取所有声明
    /// </summary>
    /// <returns></returns>
    Claim[] GetAllClaims();

    /// <summary>
    /// 判断用户是否拥有此角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    bool IsInRole(string roleId);
}