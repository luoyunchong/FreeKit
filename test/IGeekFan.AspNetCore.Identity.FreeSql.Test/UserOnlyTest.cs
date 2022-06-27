using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;

public class TestUserDbContext : IdentityUserContext<IdentityUser>
{
    public TestUserDbContext(IOptions<IdentityOptions> identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions.Value, fsql, options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }

    protected override void OnModelCreating(ICodeFirst codefirst)
    {
        base.OnModelCreating(codefirst);
        codefirst.SyncStructure(typeof(IdentityUser), typeof(IdentityRole));
    }
}

public class UserOnlyTest : IClassFixture<ScratchDatabaseFixture>
{
    private readonly ApplicationBuilder _builder;

    public UserOnlyTest(ScratchDatabaseFixture fixture)
    {
        var services = DbUtil.Create<TestUserDbContext>(fixture.Connection);

        services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
             .AddFreeSqlStores<TestUserDbContext>();

        var provider = services.BuildServiceProvider();

        _builder = new ApplicationBuilder(provider);

        //using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        //{
        //    var dbcontext = scoped.ServiceProvider.GetRequiredService<TestUserDbContext>();
        //    dbcontext.Orm.CodeFirst.IsAutoSyncStructure = true;
        //    dbcontext.Orm.CodeFirst.SyncStructure(typeof(IdentityUser), typeof(IdentityRole));
        //}
    }

    [Fact]
    public async Task EnsureStartupUsageWorks()
    {
        var userStore = _builder.ApplicationServices.GetRequiredService<IUserStore<IdentityUser>>();
        var userManager = _builder.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();

        Assert.NotNull(userStore);
        Assert.NotNull(userManager);

        const string userName = "admin";
        const string password = "[PLACEHOLDER]-1a";
        var user = new IdentityUser { UserName = userName };
        IdentityResultAssert.IsSuccess(await userManager.CreateAsync(user, password));
        IdentityResultAssert.IsSuccess(await userManager.DeleteAsync(user));
    }

    [Fact]
    public async Task FindByEmailThrowsWithTwoUsersWithSameEmail()
    {
        var userStore = _builder.ApplicationServices.GetRequiredService<IUserStore<IdentityUser>>();
        var manager = _builder.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();

        Assert.NotNull(userStore);
        Assert.NotNull(manager);

        var userA = new IdentityUser(Guid.NewGuid().ToString());
        userA.Email = "dupe@dupe.com";
        const string password = "[PLACEHOLDER]-1a";
        IdentityResultAssert.IsSuccess(await manager.CreateAsync(userA, password));
        var userB = new IdentityUser(Guid.NewGuid().ToString());
        userB.Email = "dupe@dupe.com";
        IdentityResultAssert.IsSuccess(await manager.CreateAsync(userB, password));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.FindByEmailAsync("dupe@dupe.com"));
    }
}
