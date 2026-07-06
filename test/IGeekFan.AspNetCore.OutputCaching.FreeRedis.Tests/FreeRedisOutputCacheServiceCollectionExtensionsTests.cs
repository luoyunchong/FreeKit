using Microsoft.AspNetCore.OutputCaching;
using IGeekFan.AspNetCore.OutputCaching.FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.OutputCaching.FreeRedis.Tests;

public sealed class FreeRedisOutputCacheServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFreeRedisOutputCache_RegistersOutputCacheStore()
    {
        var services = new ServiceCollection();

        services.AddFreeRedisOutputCache(options =>
        {
            options.Configuration = "127.0.0.1:6379";
            options.InstanceName = "test:";
        });

        using var provider = services.BuildServiceProvider();

        Assert.IsType<FreeRedisOutputCacheStore>(provider.GetRequiredService<IOutputCacheStore>());
        Assert.Equal("test:", provider.GetRequiredService<IOptions<FreeRedisOutputCacheOptions>>().Value.InstanceName);
    }
}
