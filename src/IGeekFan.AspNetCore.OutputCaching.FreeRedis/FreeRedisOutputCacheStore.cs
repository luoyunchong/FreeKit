using FreeRedis;
using IGeekFan.AspNetCore.OutputCaching.FreeRedis.Internal;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis;

/// <summary>
/// An <see cref="IOutputCacheStore"/> implementation backed by FreeRedis.
/// </summary>
public sealed class FreeRedisOutputCacheStore : IOutputCacheStore
{
    private const string ValueKeyPrefix = "oc:";
    private const string TagKeyPrefix = "tag:";
    private readonly IFreeRedisOutputCacheDatabase _database;
    private readonly string _instanceName;

    /// <summary>
    /// Initializes a new instance of <see cref="FreeRedisOutputCacheStore"/>.
    /// </summary>
    public FreeRedisOutputCacheStore(
        IRedisClient redisClient,
        IOptions<FreeRedisOutputCacheOptions> options)
        : this(new FreeRedisOutputCacheDatabase(redisClient), options)
    {
    }

    internal FreeRedisOutputCacheStore(
        IFreeRedisOutputCacheDatabase database,
        IOptions<FreeRedisOutputCacheOptions> options)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        ArgumentNullException.ThrowIfNull(options);

        _instanceName = options.Value.InstanceName ?? string.Empty;
    }

    /// <inheritdoc />
    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        return await _database.GetAsync(GetValueKey(key)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetAsync(
        string key,
        byte[] value,
        string[]? tags,
        TimeSpan validFor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        cancellationToken.ThrowIfCancellationRequested();

        var valueKey = GetValueKey(key);
        await _database.SetAsync(valueKey, value, validFor).ConfigureAwait(false);

        if (tags is null || tags.Length == 0)
        {
            return;
        }

        foreach (var tag in tags)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(tag))
            {
                continue;
            }

            var tagKey = GetTagKey(tag);
            await _database.AddToSetAsync(tagKey, valueKey).ConfigureAwait(false);
            await _database.ExpireAsync(tagKey, validFor).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tag);
        cancellationToken.ThrowIfCancellationRequested();

        var tagKey = GetTagKey(tag);
        var keys = await _database.GetSetMembersAsync(tagKey).ConfigureAwait(false);
        if (keys.Length > 0)
        {
            await _database.DeleteAsync(keys).ConfigureAwait(false);
        }

        await _database.DeleteAsync(tagKey).ConfigureAwait(false);
    }

    private string GetValueKey(string key) => _instanceName + ValueKeyPrefix + key;

    private string GetTagKey(string tag) => _instanceName + TagKeyPrefix + tag;
}
