// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// Provides access to the current <see cref="ICurrentUser"/>, if one is available.
/// </summary>
/// <remarks>
/// This interface should be used with caution. It relies on <see cref="System.Threading.AsyncLocal{T}" /> which can have a negative performance impact on async calls.
/// It also creates a dependency on "ambient state" which can make testing more difficult.
/// </remarks>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets or sets the current user <see cref="ICurrentUser"/>
    /// </summary>
    ICurrentUser? CurrentUser { get; set; }

    /// <summary>
    /// 临时切换当前用户，释放作用域时恢复原上下文并清理当前上下文。
    /// </summary>
    IDisposable Change(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        var previous = CurrentUser;
        CurrentUser = currentUser;
        return new CurrentUserAccessorScope(this, previous);
    }
}
