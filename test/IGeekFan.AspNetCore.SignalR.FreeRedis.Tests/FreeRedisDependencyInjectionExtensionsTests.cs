using IGeekFan.AspNetCore.SignalR.FreeRedis;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.SignalR.FreeRedis.Tests;

public sealed class FreeRedisDependencyInjectionExtensionsTests
{
    [Fact]
    public void AddFreeRedis_RegistersHubLifetimeManagerAndOptions()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSignalR().AddFreeRedis(options =>
        {
            options.Configuration = "127.0.0.1:6379";
        });

        using var provider = services.BuildServiceProvider();

        Assert.IsType<FreeRedisHubLifetimeManager<TestHub>>(provider.GetRequiredService<HubLifetimeManager<TestHub>>());
        Assert.Equal("127.0.0.1:6379", provider.GetRequiredService<IOptions<FreeRedisOptions>>().Value.Configuration);
    }

    private sealed class TestHub : Hub
    {
    }
}
