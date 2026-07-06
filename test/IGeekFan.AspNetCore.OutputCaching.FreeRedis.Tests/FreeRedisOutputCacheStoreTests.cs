using IGeekFan.AspNetCore.OutputCaching.FreeRedis;
using IGeekFan.AspNetCore.OutputCaching.FreeRedis.Internal;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis.Tests;

public sealed class FreeRedisOutputCacheStoreTests
{
    [Fact]
    public async Task SetAsync_StoresValueWithInstanceNamePrefix()
    {
        var database = new TestOutputCacheDatabase();
        var store = CreateStore(database, "test:");
        var value = new byte[] { 1, 2, 3 };

        await store.SetAsync("cache-key", value, Array.Empty<string>(), TimeSpan.FromMinutes(5), CancellationToken.None);

        Assert.Equal(value, database.Values["test:oc:cache-key"].Value);
        Assert.Equal(TimeSpan.FromMinutes(5), database.Values["test:oc:cache-key"].Expiry);
    }

    [Fact]
    public async Task GetAsync_ReturnsStoredValue()
    {
        var database = new TestOutputCacheDatabase();
        var store = CreateStore(database);
        var value = new byte[] { 4, 5, 6 };
        await database.SetAsync("oc:cache-key", value, TimeSpan.FromMinutes(5));

        var result = await store.GetAsync("cache-key", CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task SetAsync_AssociatesCacheEntryWithTags()
    {
        var database = new TestOutputCacheDatabase();
        var store = CreateStore(database);

        await store.SetAsync("cache-key", new byte[] { 1 }, new[] { "tag-a", "tag-b" }, TimeSpan.FromSeconds(30), CancellationToken.None);

        Assert.Contains("oc:cache-key", database.Sets["tag:tag-a"]);
        Assert.Contains("oc:cache-key", database.Sets["tag:tag-b"]);
        Assert.Equal(TimeSpan.FromSeconds(30), database.SetExpiries["tag:tag-a"]);
        Assert.Equal(TimeSpan.FromSeconds(30), database.SetExpiries["tag:tag-b"]);
    }

    [Fact]
    public async Task EvictByTagAsync_DeletesTaggedEntriesAndTagSet()
    {
        var database = new TestOutputCacheDatabase();
        var store = CreateStore(database);
        await store.SetAsync("first", new byte[] { 1 }, new[] { "tag-a" }, TimeSpan.FromMinutes(5), CancellationToken.None);
        await store.SetAsync("second", new byte[] { 2 }, new[] { "tag-a" }, TimeSpan.FromMinutes(5), CancellationToken.None);

        await store.EvictByTagAsync("tag-a", CancellationToken.None);

        Assert.False(database.Values.ContainsKey("oc:first"));
        Assert.False(database.Values.ContainsKey("oc:second"));
        Assert.False(database.Sets.ContainsKey("tag:tag-a"));
    }

    private static FreeRedisOutputCacheStore CreateStore(TestOutputCacheDatabase database, string? instanceName = null)
    {
        return new FreeRedisOutputCacheStore(
            database,
            Options.Create(new FreeRedisOutputCacheOptions { InstanceName = instanceName }));
    }

    private sealed class TestOutputCacheDatabase : IFreeRedisOutputCacheDatabase
    {
        public Dictionary<string, (byte[] Value, TimeSpan Expiry)> Values { get; } = new();

        public Dictionary<string, HashSet<string>> Sets { get; } = new();

        public Dictionary<string, TimeSpan> SetExpiries { get; } = new();

        public Task<byte[]?> GetAsync(string key)
        {
            return Task.FromResult(Values.TryGetValue(key, out var entry) ? entry.Value : null);
        }

        public Task SetAsync(string key, byte[] value, TimeSpan expiry)
        {
            Values[key] = (value, expiry);
            return Task.CompletedTask;
        }

        public Task AddToSetAsync(string key, string value)
        {
            if (!Sets.TryGetValue(key, out var members))
            {
                members = new HashSet<string>(StringComparer.Ordinal);
                Sets[key] = members;
            }

            members.Add(value);
            return Task.CompletedTask;
        }

        public Task<string[]> GetSetMembersAsync(string key)
        {
            return Task.FromResult(Sets.TryGetValue(key, out var members) ? members.ToArray() : Array.Empty<string>());
        }

        public Task ExpireAsync(string key, TimeSpan expiry)
        {
            SetExpiries[key] = expiry;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(params string[] keys)
        {
            foreach (var key in keys)
            {
                Values.Remove(key);
                Sets.Remove(key);
            }

            return Task.CompletedTask;
        }
    }
}
