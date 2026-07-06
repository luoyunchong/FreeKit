// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeRedis;

namespace IGeekFan.AspNetCore.SignalR.FreeRedis;

/// <summary>
/// Options used to configure <see cref="FreeRedisHubLifetimeManager{THub}"/>.
/// </summary>
public class FreeRedisOptions
{
    /// <summary>
    /// Gets or sets the FreeRedis connection string.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Gets or sets an existing FreeRedis client.
    /// </summary>
    public IRedisClient? RedisClient { get; set; }

    /// <summary>
    /// Gets or sets the Redis connection factory.
    /// </summary>
    public Func<TextWriter, Task<IRedisClient>>? ConnectionFactory { get; set; }

    internal async Task<IRedisClient> ConnectAsync(TextWriter log)
    {
        if (RedisClient is not null)
        {
            return RedisClient;
        }

        var factory = ConnectionFactory;
        if (factory is not null)
        {
            return await factory(log);
        }

        if (string.IsNullOrWhiteSpace(Configuration))
        {
            throw new InvalidOperationException($"{nameof(Configuration)} must be provided.");
        }

        return new RedisClient(Configuration);
    }
}
