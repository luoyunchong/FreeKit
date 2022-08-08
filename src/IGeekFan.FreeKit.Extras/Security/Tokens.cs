// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 访问令牌
/// </summary>
[Serializable]
public class Tokens
{
    public Tokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
    }

    /// <summary>
    /// 授权接口调用凭证
    /// </summary>
    /// <value></value>
    public string AccessToken { get; private set; }

    /// <summary>
    /// 用户刷新AccessToken
    /// </summary>
    /// <value></value>
    public string RefreshToken { get; private set; }

    public override string ToString()
    {
        return $"Tokens AccessToken:{AccessToken},RefreshToken:{RefreshToken}";
    }
}
