// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeRedis;
using IGeekFan.AspNetCore.SignalR.FreeRedis;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring FreeRedis-based scale-out for a SignalR Server in an <see cref="ISignalRServerBuilder" />.
/// </summary>
public static class FreeRedisDependencyInjectionExtensions
{
    /// <summary>
    /// Adds scale-out to a <see cref="ISignalRServerBuilder"/>, using a shared Redis server.
    /// </summary>
    public static ISignalRServerBuilder AddFreeRedis(this ISignalRServerBuilder signalrBuilder)
    {
        return AddFreeRedis(signalrBuilder, o => { });
    }

    /// <summary>
    /// Adds scale-out to a <see cref="ISignalRServerBuilder"/>, using a shared Redis server.
    /// </summary>
    public static ISignalRServerBuilder AddFreeRedis(this ISignalRServerBuilder signalrBuilder, string redisConnectionString)
    {
        return AddFreeRedis(signalrBuilder, o =>
        {
            o.Configuration = redisConnectionString;
        });
    }

    /// <summary>
    /// Adds scale-out to a <see cref="ISignalRServerBuilder"/>, using an existing FreeRedis client.
    /// </summary>
    public static ISignalRServerBuilder AddFreeRedis(this ISignalRServerBuilder signalrBuilder, IRedisClient redisClient)
    {
        return AddFreeRedis(signalrBuilder, o =>
        {
            o.RedisClient = redisClient;
        });
    }

    /// <summary>
    /// Adds scale-out to a <see cref="ISignalRServerBuilder"/>, using a shared Redis server.
    /// </summary>
    public static ISignalRServerBuilder AddFreeRedis(this ISignalRServerBuilder signalrBuilder, Action<FreeRedisOptions> configure)
    {
        signalrBuilder.Services.Configure(configure);
        signalrBuilder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(FreeRedisHubLifetimeManager<>));
        return signalrBuilder;
    }

    /// <summary>
    /// Adds scale-out to a <see cref="ISignalRServerBuilder"/>, using a shared Redis server.
    /// </summary>
    public static ISignalRServerBuilder AddFreeRedis(this ISignalRServerBuilder signalrBuilder, string redisConnectionString, Action<FreeRedisOptions> configure)
    {
        return AddFreeRedis(signalrBuilder, o =>
        {
            o.Configuration = redisConnectionString;
            configure(o);
        });
    }
}
