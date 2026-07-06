namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis;

/// <summary>
/// Configuration options for <see cref="FreeRedisOutputCacheStore"/>.
/// </summary>
public sealed class FreeRedisOutputCacheOptions
{
    /// <summary>
    /// Gets or sets the Redis configuration string used to create a <c>FreeRedis.RedisClient</c>.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Gets or sets a prefix used for all keys in Redis.
    /// </summary>
    public string? InstanceName { get; set; }
}
