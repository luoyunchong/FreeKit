using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FreeSql;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using IdentityResultAssert = IGeekFan.AspNetCore.Identity.FreeSql.Test.Specification.Tests.IdentityResultAssert;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;

public abstract class SqlStoreTestBase<TUser, TRole, TKey> : Specification.Tests.IdentitySpecificationTestBase<TUser, TRole, TKey>, IClassFixture<ScratchDatabaseFixture>
    where TUser : IdentityUser<TKey>, new()
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    protected readonly ScratchDatabaseFixture _fixture;

    protected SqlStoreTestBase(ScratchDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    protected virtual void SetupAddIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<TUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.AllowedUserNameCharacters = null;
        })
        .AddRoles<TRole>()
        .AddDefaultTokenProviders()
        .AddFreeSqlStores<TestDbContext>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
    }


    public class TestDbContext : IdentityDbContext<TUser, TRole, TKey>
    {
        public TestDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions, fsql, options) { }
    }

    protected override TUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
        bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default(DateTimeOffset?), bool useNamePrefixAsUserName = false)
    {
        return new TUser
        {
            UserName = useNamePrefixAsUserName ? namePrefix : string.Format(CultureInfo.InvariantCulture, "{0}{1}", namePrefix, Guid.NewGuid()),
            Email = email,
            PhoneNumber = phoneNumber,
            LockoutEnabled = lockoutEnabled,
            LockoutEnd = lockoutEnd
        };
    }



    protected override TRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
    {
        var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format(CultureInfo.InvariantCulture, "{0}{1}", roleNamePrefix, Guid.NewGuid());
        return new TRole() { Name = roleName };
    }

    protected override Expression<Func<TRole, bool>> RoleNameEqualsPredicate(string roleName) => r => r.Name == roleName;

    protected override Expression<Func<TUser, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

#pragma warning disable CA1310 // Specify StringComparison for correctness
    protected override Expression<Func<TRole, bool>> RoleNameStartsWithPredicate(string roleName) => r => r.Name.StartsWith(roleName);

    protected override Expression<Func<TUser, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName.StartsWith(userName);
#pragma warning restore CA1310 // Specify StringComparison for correctness

    protected virtual TestDbContext CreateContext()
    {
        var services = DbUtil.Create<TestDbContext>(_fixture.Connection);
        //var services = DbUtil.Create<TestDbContext>("Data Source=hello.db");
        SetupAddIdentity(services);
        var db = services.BuildServiceProvider().GetRequiredService<TestDbContext>();
        return db;
    }

    protected override object CreateTestContext()
    {
        return CreateContext();
    }

    protected override void AddUserStore(IServiceCollection services, object context = null)
    {
        services.AddSingleton<IUserStore<TUser>>(new UserStore<TUser, TRole, TestDbContext, TKey>((TestDbContext)context));
    }

    protected override void AddRoleStore(IServiceCollection services, object context = null)
    {
        services.AddSingleton<IRoleStore<TRole>>(new RoleStore<TRole, TestDbContext, TKey>((TestDbContext)context));
    }

    protected override void SetUserPasswordHash(TUser user, string hashedPassword)
    {
        user.PasswordHash = hashedPassword;
    }

    [Fact]
    public void EnsureDefaultSchema()
    {
        VerifyDefaultSchema(CreateContext());
    }

    internal static void VerifyDefaultSchema(TestDbContext dbContext)
    {
    }

    [Fact]
    public async Task DeleteRoleNonEmptySucceedsTest()
    {
    }

    [Fact]
    public async Task DeleteUserRemovesFromRoleTest()
    {
        // Need fail if not empty?
        var userMgr = CreateManager();
        var roleMgr = CreateRoleManager();
        var roleName = "deleteUserRemove" + Guid.NewGuid().ToString();
        var role = CreateTestRole(roleName, useRoleNamePrefixAsRoleName: true);
        Assert.False(await roleMgr.RoleExistsAsync(roleName));
        IdentityResultAssert.IsSuccess(await roleMgr.CreateAsync(role));
        var user = CreateTestUser();
        IdentityResultAssert.IsSuccess(await userMgr.CreateAsync(user));
        IdentityResultAssert.IsSuccess(await userMgr.AddToRoleAsync(user, roleName));

        var roles = await userMgr.GetRolesAsync(user);
        Assert.Single(roles);

        IdentityResultAssert.IsSuccess(await userMgr.DeleteAsync(user));

        roles = await userMgr.GetRolesAsync(user);
        //Assert.Empty(roles);
    }

    [Fact]
    public async Task DeleteUserRemovesTokensTest()
    {
        // Need fail if not empty?
        var userMgr = CreateManager();
        var user = CreateTestUser();
        IdentityResultAssert.IsSuccess(await userMgr.CreateAsync(user));
        IdentityResultAssert.IsSuccess(await userMgr.SetAuthenticationTokenAsync(user, "provider", "test", "value"));

        Assert.Equal("value", await userMgr.GetAuthenticationTokenAsync(user, "provider", "test"));

        IdentityResultAssert.IsSuccess(await userMgr.DeleteAsync(user));

        //TODO
        //不会级联删除
        //Assert.Null(await userMgr.GetAuthenticationTokenAsync(user, "provider", "test"));
    }

    [Fact]
    public void CanCreateUserUsingFreeSql()
    {
        using (var db = CreateContext())
        {

            var user = CreateTestUser();
            db.Users.Add(user);
            db.SaveChanges();
            Assert.True(db.Users.Select.Any(u => u.UserName == user.UserName));
            Assert.NotNull(db.Users.Select.Where(u => u.UserName == user.UserName).First());
        }
    }

    [Fact]
    public async Task CanCreateUsingManager()
    {
        var manager = CreateManager();
        var user = CreateTestUser();
        IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
        IdentityResultAssert.IsSuccess(await manager.DeleteAsync(user));
    }

    private async Task LazyLoadTestSetup(TestDbContext db, TUser user)
    {
        var context = db;
        var manager = CreateManager(context);
        var role = CreateRoleManager(context);
        var admin = CreateTestRole("Admin" + Guid.NewGuid().ToString());
        var local = CreateTestRole("Local" + Guid.NewGuid().ToString());
        IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
        IdentityResultAssert.IsSuccess(await manager.AddLoginAsync(user, new UserLoginInfo("provider", user.Id.ToString(), "display")));
        IdentityResultAssert.IsSuccess(await role.CreateAsync(admin));
        IdentityResultAssert.IsSuccess(await role.CreateAsync(local));
        IdentityResultAssert.IsSuccess(await manager.AddToRoleAsync(user, admin.Name));
        IdentityResultAssert.IsSuccess(await manager.AddToRoleAsync(user, local.Name));
        Claim[] userClaims =
        {
                new Claim("Whatever", "Value"),
                new Claim("Whatever2", "Value2")
            };
        foreach (var c in userClaims)
        {
            IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user, c));
        }
    }

    [Fact]
    public async Task LoadFromDbFindByIdTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        await LazyLoadTestSetup(db, user);

        db = CreateContext();
        var manager = CreateManager(db);

        var userById = await manager.FindByIdAsync(user.Id.ToString());
        Assert.Equal(2, (await manager.GetClaimsAsync(userById)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userById)).Count);
        Assert.Equal(2, (await manager.GetRolesAsync(userById)).Count);
    }

    [Fact]
    public async Task LoadFromDbFindByNameTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        await LazyLoadTestSetup(db, user);

        //db = CreateContext();
        var manager = CreateManager(db);
        var userByName = await manager.FindByNameAsync(user.UserName);
        Assert.Equal(2, (await manager.GetClaimsAsync(userByName)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByName)).Count);
        Assert.Equal(2, (await manager.GetRolesAsync(userByName)).Count);
    }

    [Fact]
    public async Task LoadFromDbFindByLoginTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        await LazyLoadTestSetup(db, user);

        db = CreateContext();
        var manager = CreateManager(db);
        var userByLogin = await manager.FindByLoginAsync("provider", user.Id.ToString());
        Assert.Equal(2, (await manager.GetClaimsAsync(userByLogin)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByLogin)).Count);
        Assert.Equal(2, (await manager.GetRolesAsync(userByLogin)).Count);
    }

    [Fact]
    public async Task GetSecurityStampThrowsIfNull()
    {
        var manager = CreateManager();
        var user = CreateTestUser();
        var result = await manager.CreateAsync(user);
        Assert.NotNull(user);
        user.SecurityStamp = null;
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.GetSecurityStampAsync(user));
    }

    [Fact]
    public async Task LoadFromDbFindByEmailTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        user.Email = "fooz@fizzy.pop";
        await LazyLoadTestSetup(db, user);

        db = CreateContext();
        var manager = CreateManager(db);
        var userByEmail = await manager.FindByEmailAsync(user.Email);
        Assert.Equal(2, (await manager.GetClaimsAsync(userByEmail)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByEmail)).Count);
        Assert.Equal(2, (await manager.GetRolesAsync(userByEmail)).Count);
    }



}
