// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 统一ClaimTypes
/// </summary>
public static class FreeKitClaimTypes
{
    /// <summary>
    /// 租户
    /// </summary>
    public const string TenantName = "tenantname";
    /// <summary>
    /// 租户Id
    /// </summary>
    public const string TenantId = "tenantid";
    /// <summary>
    /// 用户Id
    /// </summary>
    public const string NameIdentifier = ClaimTypes.NameIdentifier;
    /// <summary>
    /// 登录名
    /// </summary>
    public const string UserName = ClaimTypes.Name;
    /// <summary>
    /// 邮件
    /// </summary>
    public const string Email = ClaimTypes.Email;
    /// <summary>
    /// 角色
    /// </summary>
    public const string Role = ClaimTypes.Role;
    /// <summary>
    /// 手机号
    /// </summary>
    public const string PhoneNumber = ClaimTypes.MobilePhone;
    /// <summary>
    /// 姓名
    /// </summary>
    public const string Name = ClaimTypes.GivenName;
}