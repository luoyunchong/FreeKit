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
}