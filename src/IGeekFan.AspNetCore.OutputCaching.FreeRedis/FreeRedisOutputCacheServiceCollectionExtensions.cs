using FreeRedis;
using IGeekFan.AspNetCore.OutputCaching.FreeRedis;
using IGeekFan.AspNetCore.OutputCaching.FreeRedis.Internal;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis;

/// <summary>
/// Extension methods for configuring FreeRedis output caching services.
/// </summary>
public static class FreeRedisOutputCacheServiceCollectionExtensions
{
    /// <summary>
    /// Configures ASP.NET Core output caching to use FreeRedis.
    /// </summary>
    public static IServiceCollection AddFreeRedisOutputCache(
        this IServiceCollection services,
        Action<FreeRedisOutputCacheOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.Configure(setupAction);
        services.TryAddSingleton<IFreeRedisOutputCacheDatabase>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FreeRedisOutputCacheOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.Configuration))
            {
                throw new InvalidOperationException($"{nameof(FreeRedisOutputCacheOptions.Configuration)} must be provided.");
            }

            return new FreeRedisOutputCacheDatabase(new RedisClient(options.Configuration));
        });
        services.AddSingleton<IOutputCacheStore>(serviceProvider => new FreeRedisOutputCacheStore(
            serviceProvider.GetRequiredService<IFreeRedisOutputCacheDatabase>(),
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FreeRedisOutputCacheOptions>>()));

        return services;
    }

    /// <summary>
    /// Configures ASP.NET Core output caching to use an existing FreeRedis client.
    /// </summary>
    public static IServiceCollection AddFreeRedisOutputCache(
        this IServiceCollection services,
        IRedisClient redisClient,
        Action<FreeRedisOutputCacheOptions>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(redisClient);

        if (setupAction is not null)
        {
            services.Configure(setupAction);
        }

        services.AddSingleton<IFreeRedisOutputCacheDatabase>(new FreeRedisOutputCacheDatabase(redisClient));
        services.AddSingleton<IOutputCacheStore>(serviceProvider => new FreeRedisOutputCacheStore(
            serviceProvider.GetRequiredService<IFreeRedisOutputCacheDatabase>(),
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FreeRedisOutputCacheOptions>>()));

        return services;
    }
}
