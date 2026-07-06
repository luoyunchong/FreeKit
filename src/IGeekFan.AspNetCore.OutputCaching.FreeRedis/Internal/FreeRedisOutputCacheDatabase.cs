using FreeRedis;

namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis.Internal;

internal sealed class FreeRedisOutputCacheDatabase : IFreeRedisOutputCacheDatabase
{
    private readonly IRedisClient _redisClient;

    public FreeRedisOutputCacheDatabase(IRedisClient redisClient)
    {
        _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
    }

    public async Task<byte[]?> GetAsync(string key)
    {
        return await _redisClient.GetAsync<byte[]>(key).ConfigureAwait(false);
    }

    public async Task SetAsync(string key, byte[] value, TimeSpan expiry)
    {
        await _redisClient.SetAsync(key, value, expiry).ConfigureAwait(false);
    }

    public async Task AddToSetAsync(string key, string value)
    {
        await _redisClient.SAddAsync(key, value).ConfigureAwait(false);
    }

    public async Task<string[]> GetSetMembersAsync(string key)
    {
        return await _redisClient.SMembersAsync(key).ConfigureAwait(false);
    }

    public async Task ExpireAsync(string key, TimeSpan expiry)
    {
        await _redisClient.ExpireAsync(key, expiry).ConfigureAwait(false);
    }

    public async Task DeleteAsync(params string[] keys)
    {
        if (keys.Length == 0)
        {
            return;
        }

        await _redisClient.DelAsync(keys).ConfigureAwait(false);
    }
}
