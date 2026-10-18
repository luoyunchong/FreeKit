using System.Security.Claims;
using Autofac;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Security;
using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IGeekFan.FreeKit.PackageCompatibility.Tests;

public class PackageCompatibilityTests
{
    [Fact]
    public void LegacyAccessorWithoutChangeUsesDefaultNestedScopes()
    {
        var previous = User("previous");
        var outer = User("outer");
        var inner = User("inner");
        ICurrentUserAccessor accessor = new LegacyAccessor { CurrentUser = previous };

        using (accessor.Change(outer))
        {
            using (accessor.Change(inner)) Assert.Same(inner, accessor.CurrentUser);
            Assert.Same(outer, accessor.CurrentUser);
        }

        Assert.Same(previous, accessor.CurrentUser);
    }

    [Fact]
    public async Task MiddlewareUsesDefaultInterfaceScopeAndRestoresOnFailure()
    {
        var previous = User("previous");
        var request = User("request");
        var accessor = new LegacyAccessor { CurrentUser = previous };
        var services = new ServiceCollection();
        services.AddSingleton<ICurrentUserAccessor>(accessor);
        services.AddSingleton<ICurrentUser>(request);
        using var provider = services.BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = provider };
        var middleware = new CurrentUserAccessorMiddleware(async _ =>
        {
            await Task.Yield();
            Assert.Same(request, accessor.CurrentUser);
            throw new InvalidOperationException("synthetic request failure");
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.Same(previous, accessor.CurrentUser);
    }

    [Fact]
    public void DefaultInterfaceScopeCanBeDisposedTwice()
    {
        var previous = User("previous");
        ICurrentUserAccessor accessor = new LegacyAccessor { CurrentUser = previous };
        var scope = accessor.Change(User("request"));
        scope.Dispose();
        var later = User("later");
        accessor.CurrentUser = later;

        scope.Dispose();

        Assert.Same(later, accessor.CurrentUser);
    }

    [Fact]
    public async Task BuiltInScopeRestoresParentAndClearsInheritedChildUser()
    {
        var accessor = new CurrentUserAccessor();
        var parent = User("parent");
        var request = User("request");
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using (accessor.Change(parent))
        {
            Task<ICurrentUser?> child;
            using (accessor.Change(request))
            {
                child = Task.Run(async () =>
                {
                    await release.Task;
                    return accessor.CurrentUser;
                });
                await Task.Yield();
                Assert.Same(request, accessor.CurrentUser);
            }

            Assert.Same(parent, accessor.CurrentUser);
            release.SetResult();
            Assert.Null(await child);
        }

        Assert.Null(accessor.CurrentUser);
    }

    [Fact]
    public void RuntimeCanBeRegisteredTwiceWithoutDatabaseServices()
    {
        var services = new ServiceCollection();
        services.AddFreeKitRuntime().AddFreeKitRuntime();
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<ICurrentUser>());
        Assert.Same(provider.GetRequiredService<ICurrentUserAccessor>(), provider.GetRequiredService<ICurrentUserAccessor>());
        Assert.Null(provider.GetService<IFreeSql>());
        Assert.Null(provider.GetService<FreeSql.IUnitOfWorkManager>());
    }

    [Fact]
    public void ModuleServicesPreserveCoreThenOverrideWithoutMvc()
    {
        var composition = ModuleComposition.Create(
            new Dictionary<string, Type> { ["sample"] = typeof(CoreModule) },
            new Dictionary<string, Type> { ["sample"] = typeof(OverrideModule) });
        var services = new ServiceCollection();
        composition.RegisterServices(services, new ConfigurationBuilder().Build(), includeControllers: false);
        using var provider = services.BuildServiceProvider();

        Assert.Equal(new[] { "core", "override" }, provider.GetServices<Marker>().Select(marker => marker.Name));
        Assert.Equal(2, provider.GetServices<ModuleInfo>().Count());
        Assert.Null(provider.GetService<IActionDescriptorCollectionProvider>());
        Assert.Single(composition.Assemblies);
    }

    [Fact]
    public void UnknownModuleOverrideIsRejectedBeforeRegistration()
    {
        Assert.Throws<ArgumentException>(() => ModuleComposition.Create(
            new Dictionary<string, Type> { ["sample"] = typeof(CoreModule) },
            new Dictionary<string, Type> { ["missing"] = typeof(OverrideModule) }));
    }

    [Fact]
    public void SealedServiceCanStillResolveAsSelf()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new UnitOfWorkModule(new[] { typeof(SealedProbeService).Assembly }));
        using var container = builder.Build();

        Assert.Equal("ready", container.Resolve<SealedProbeService>().Value);
    }

    private static ICurrentUser User(string id) => new CurrentUser(new StaticHttpContextAccessor
    {
        HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(FreeKitClaimTypes.NameIdentifier, id) }))
        }
    });

    // Deliberately implements the published 0.0.559 contract only.
    private sealed class LegacyAccessor : ICurrentUserAccessor
    {
        public ICurrentUser? CurrentUser { get; set; }
    }

    private sealed class StaticHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    public sealed record Marker(string Name);

    public sealed class CoreModule : IModuleStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddSingleton(new Marker("core"));
        public void Configure(WebApplication application, IWebHostEnvironment environment) { }
    }

    public sealed class OverrideModule : IModuleStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddSingleton(new Marker("override"));
        public void Configure(WebApplication application, IWebHostEnvironment environment) { }
    }
}

public sealed class SealedProbeService
{
    public string Value => "ready";
}
