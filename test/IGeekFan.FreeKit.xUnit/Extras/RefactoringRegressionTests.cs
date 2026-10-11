using System.Security.Claims;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.MultiTenancy;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Extras;

public class RefactoringRegressionTests
{
    [Fact]
    public void IntAuditRepositoryUsesIntUserKeys()
    {
        var services = new ServiceCollection();

        services.AddAuditRepository(typeof(int));

        var descriptor = services.Single(service =>
            service.ServiceType == typeof(IAuditBaseRepository<>));

        Assert.Equal(typeof(AuditIntRepository<>), descriptor.ImplementationType);
    }

    [Fact]
    public void InvalidNumericUserIdReturnsNull()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(FreeKitClaimTypes.NameIdentifier, "not-a-number")
        }));

        Assert.False(principal.FindUserIdToInt().HasValue);
        Assert.False(principal.FindUserIdToLong().HasValue);
        Assert.Equal(0, principal.FindUserId<int>());
        Assert.Equal(0L, principal.FindUserId<long>());
    }

    [Fact]
    public void IgnoreTenantScopeRestoresNestedState()
    {
        using (FreeSqlMultiTenancyExtensions.BeginIgnoreTenant())
        {
            Assert.True(FreeSqlMultiTenancyExtensions.IsTenantFilterDisabled);

            using (FreeSqlMultiTenancyExtensions.BeginIgnoreTenant())
            {
                Assert.True(FreeSqlMultiTenancyExtensions.IsTenantFilterDisabled);
            }

            Assert.True(FreeSqlMultiTenancyExtensions.IsTenantFilterDisabled);
        }

        Assert.False(FreeSqlMultiTenancyExtensions.IsTenantFilterDisabled);
    }

    [Fact]
    public void UnitOfWorkManagerIsAvailableThroughItsInterface()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFreeSql>(_ => new FreeSqlBuilder()
            .UseConnectionString(DataType.Sqlite, "Data Source=:memory:")
            .Build());
        services.AddUnitOfWorkManager();

        using var provider = services.BuildServiceProvider();
        var manager = provider.GetRequiredService<IUnitOfWorkManager>();

        Assert.IsType<UnitOfWorkManager>(manager);
    }

    [Fact]
    public void CurrentUserIntIdIsConvertedWithoutBoxingMismatch()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(FreeKitClaimTypes.NameIdentifier, "42")
            }))
        };

        var currentUser = new CurrentUser(new HttpContextAccessor { HttpContext = httpContext });

        Assert.Equal(42, currentUser.FindUserId<int>());
        Assert.Equal("42", currentUser.Id);
    }

    [Fact]
    public void MultiTenancyRegistersResolversFromConfiguredOptions()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancy();

        using var provider = services.BuildServiceProvider();
        var resolvers = provider.GetServices<ITenantResolver>().ToArray();

        Assert.Equal(3, resolvers.Length);
        Assert.Contains(resolvers, resolver => resolver is ClaimTenantResolver);
        Assert.Contains(resolvers, resolver => resolver is HeaderTenantResolver);
        Assert.Contains(resolvers, resolver => resolver is QueryStringTenantResolver);
    }
}
