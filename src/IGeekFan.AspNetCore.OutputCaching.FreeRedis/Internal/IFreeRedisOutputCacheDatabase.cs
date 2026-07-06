namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis.Internal;

/// <summary>
/// Minimal Redis operations needed by the output cache store.
/// </summary>
internal interface IFreeRedisOutputCacheDatabase
{
    Task<byte[]?> GetAsync(string key);

    Task SetAsync(string key, byte[] value, TimeSpan expiry);

    Task AddToSetAsync(string key, string value);

    Task<string[]> GetSetMembersAsync(string key);

    Task ExpireAsync(string key, TimeSpan expiry);

    Task DeleteAsync(params string[] keys);
}
