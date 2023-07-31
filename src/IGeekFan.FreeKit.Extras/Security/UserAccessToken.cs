// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 访问令牌
/// </summary>
[Serializable]
public class UserAccessToken
{
    public UserAccessToken(string accessToken, string refreshToken, int expiresIn, string tokenType,
        int refreshExpiresIn)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
        TokenType = tokenType;
        RefreshExpiresIn = refreshExpiresIn;
    }

    /// <summary>
    /// 授权接口调用凭证
    /// </summary>
    /// <value></value>
    [Required]
    public string AccessToken { get; private set; }

    /// <summary>
    /// 用户刷新AccessToken
    /// </summary>
    /// <value></value>
    [Required]
    public string RefreshToken { get; private set; }

    /// <summary>
    /// 过期时间：单位为秒
    /// </summary>
    public int ExpiresIn { get; private set; }

    /// <summary>
    /// Token 类型
    /// </summary>
    [Required]
    public string TokenType { get; private set; }

    /// <summary>
    ///  刷新Token过期时间
    /// </summary>
    public int RefreshExpiresIn { get; private set; }
}