// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using FreeRedis;
using IGeekFan.AspNetCore.DataProtection.FreeRedis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.DataProtection;

/// <summary>
/// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
/// </summary>
public static class FreeRedisDataProtectionBuilderExtensions
{
    private const string DataProtectionKeysName = "DataProtection-Keys";

    /// <summary>
    /// Configures the data protection system to persist keys to specified key in Redis database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="databaseFactory">The delegate used to create <see cref="IRedisClient"/> instances.</param>
    /// <param name="key">The key used to store key list.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToStackExchangeRedis(this IDataProtectionBuilder builder, Func<IRedisClient> databaseFactory, string key)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        if (databaseFactory == null)
        {
            throw new ArgumentNullException(nameof(databaseFactory));
        }
        return PersistKeysToStackExchangeRedisInternal(builder, databaseFactory, key);
    }

    /// <summary>
    /// Configures the data protection system to persist keys to the default key ('DataProtection-Keys') in Redis database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="connectionMultiplexer">The <see cref="IRedisClient"/> for database access.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToStackExchangeRedis(this IDataProtectionBuilder builder, IRedisClient connectionMultiplexer)
    {
        return PersistKeysToStackExchangeRedis(builder, connectionMultiplexer, DataProtectionKeysName);
    }

    /// <summary>
    /// Configures the data protection system to persist keys to the specified key in Redis database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="connectionMultiplexer">The <see cref="IRedisClient"/> for database access.</param>
    /// <param name="key">The key used to store key list.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToStackExchangeRedis(this IDataProtectionBuilder builder, IRedisClient connectionMultiplexer, string key)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        if (connectionMultiplexer == null)
        {
            throw new ArgumentNullException(nameof(connectionMultiplexer));
        }
        return PersistKeysToStackExchangeRedisInternal(builder, () => connectionMultiplexer.GetDatabase(), key);
    }

    private static IDataProtectionBuilder PersistKeysToStackExchangeRedisInternal(IDataProtectionBuilder builder, Func<IRedisClient> databaseFactory, string key)
    {
        builder.Services.Configure<KeyManagementOptions>(options =>
        {
            options.XmlRepository = new RedisXmlRepository(databaseFactory, key);
        });
        return builder;
    }
}