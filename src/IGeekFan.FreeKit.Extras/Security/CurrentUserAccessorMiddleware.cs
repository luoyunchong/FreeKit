// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 提供一个给当前<see cref="ICurrentUserAccessor"/>赋值<see cref="ICurrentUser"/>的实现。
/// </summary>
public class CurrentUserAccessorMiddleware
{
    private readonly RequestDelegate _next;
    public CurrentUserAccessorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 通过中间件给当前<see cref="ICurrentUserAccessor"/>赋值<see cref="ICurrentUser"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var currentAccessor = context.RequestServices.GetService<ICurrentUserAccessor>();
        if (currentAccessor != null) currentAccessor.CurrentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
        await _next.Invoke(context);
        if (currentAccessor != null) currentAccessor.CurrentUser = null;
    }
}