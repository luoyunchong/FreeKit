// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// 增加<see cref="ICurrentUserAccessor"/>和<see cref="CurrentUserAccessorMiddleware"/>的扩展方法
/// </summary>
public static class CurrentUserAccessorExtensions
{
    /// <summary>
    /// Adds a default implementation for the <see cref="ICurrentUser"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddTransient<ICurrentUser, CurrentUser>();
        services.TryAddTransient(typeof(ICurrentUser<>), typeof(CurrentUser<>));
        return services;
    }

    /// <summary>
    /// Adds a default implementation for the <see cref="ICurrentUserAccessor"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCurrentUserAccessor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<ICurrentUserAccessor, CurrentUserAccessor>();
        return services;
    }

    public static IApplicationBuilder UseCurrentUserAccessor(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CurrentUserAccessorMiddleware>();
    }
}