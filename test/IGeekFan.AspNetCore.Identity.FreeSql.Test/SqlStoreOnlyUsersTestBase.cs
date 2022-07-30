// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using FreeSql;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FreeSql.DataAnnotations;
using IdentityResultAssert = IGeekFan.AspNetCore.Identity.FreeSql.Test.Specification.Tests.IdentityResultAssert;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;

[Table(Name = "AspNetUsers")]
public class AppUser : IdentityUser<Guid>
{

}
public class SqlStoreOnlyUserTest : SqlStoreOnlyUsersTestBase<AppUser, Guid>
{
}

public abstract class SqlStoreOnlyUsersTestBase<TUser, TKey> : Specification.Tests.UserManagerSpecificationTestBase<TUser, TKey>
    where TUser : IdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{

    public class TestUserDbContext : IdentityUserContext<TUser, TKey>
    {
        public TestUserDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions, fsql, options) { }
    }

    protected override TUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
        bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default, bool useNamePrefixAsUserName = false)
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

    protected override Expression<Func<TUser, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

#pragma warning disable CA1310 // Specify StringComparison for correctness
    protected override Expression<Func<TUser, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName.StartsWith(userName);
#pragma warning restore CA1310 // Specify StringComparison for correctness

    private TestUserDbContext CreateContext(string dbname = "")
    {
        if (dbname == "")
        {
            dbname = $"DataSource=D{Guid.NewGuid()}.db";
        }
        var services = DbUtil.CreateCustomPocoContext<TestUserDbContext>(dbname);
        var db = services.BuildServiceProvider().GetRequiredService<TestUserDbContext>();
        return db;
    }

    protected override object CreateTestContext()
    {
        return CreateContext($"DataSource=D{Guid.NewGuid()}.db");
    }

    protected override void AddUserStore(IServiceCollection services, object context = null)
    {
        services.AddSingleton<IUserStore<TUser>>(new UserOnlyStore<TUser, TestUserDbContext, TKey>((TestUserDbContext)context));
    }

    protected override void SetUserPasswordHash(TUser user, string hashedPassword)
    {
        user.PasswordHash = hashedPassword;
    }

    [Fact]
    public void EnsureDefaultSchema()
    {
       // VerifyDefaultSchema(CreateContext());
    }

    internal static void VerifyDefaultSchema(TestUserDbContext dbContext)
    {
        using var sqlConn = dbContext.Orm.Ado.MasterPool.Get().Value;
        dbContext.Orm.CodeFirst.SyncStructure(
            typeof(IdentityUser),
            typeof(IdentityRole),
            typeof(IdentityUserRole<Guid>),
            typeof(IdentityUserClaim<Guid>),
            typeof(IdentityUserLogin<Guid>),
            typeof(IdentityUserToken<Guid>)

            );
        using (var db = new SqliteConnection(sqlConn.ConnectionString))
        {
            db.Open();
           // Assert.True(DbUtil.VerifyColumns(db, "AspNetUsers", "Id", "UserName", "Email", "PasswordHash", "SecurityStamp",
           //     "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled",
            //    "LockoutEnd", "AccessFailedCount", "ConcurrencyStamp", "NormalizedUserName", "NormalizedEmail"));
            Assert.False(DbUtil.VerifyColumns(db, "AspNetRoles", "Id", "Name", "NormalizedName", "ConcurrencyStamp"));
            Assert.False(DbUtil.VerifyColumns(db, "AspNetUserRoles", "UserId", "RoleId"));
            Assert.True(DbUtil.VerifyColumns(db, "AspNetUserClaims", "Id", "UserId", "ClaimType", "ClaimValue"));
            Assert.True(DbUtil.VerifyColumns(db, "AspNetUserLogins", "UserId", "ProviderKey", "LoginProvider", "ProviderDisplayName"));
            Assert.True(DbUtil.VerifyColumns(db, "AspNetUserTokens", "UserId", "LoginProvider", "Name", "Value"));

            DbUtil.VerifyIndex(db, "AspNetUsers", "UserNameIndex", isUnique: true);

            DbUtil.VerifyMaxLength(dbContext, "AspNetUsers", 256, "UserName", "Email", "NormalizeUserName", "NormalizeEmail");

            db.Close();
        }
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

        Assert.Null(await userMgr.GetAuthenticationTokenAsync(user, "provider", "test"));
    }

    [Fact]
    public void CanCreateUserUsingEF()
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

    private async Task LazyLoadTestSetup(TestUserDbContext db, TUser user)
    {
        var manager = CreateManager(db);
        IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
        IdentityResultAssert.IsSuccess(await manager.AddLoginAsync(user, new UserLoginInfo("provider", user.Id.ToString(), "display")));
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

        var manager = CreateManager(db);

        var userById = await manager.FindByIdAsync(user.Id.ToString());
        Assert.Equal(2, (await manager.GetClaimsAsync(userById)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userById)).Count);
        //     Assert.Equal(2, (await manager.GetRolesAsync(userById)).Count);
    }

    [Fact]
    public async Task LoadFromDbFindByNameTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        await LazyLoadTestSetup(db, user);

        var manager = CreateManager(db);
        var userByName = await manager.FindByNameAsync(user.UserName);
        Assert.Equal(2, (await manager.GetClaimsAsync(userByName)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByName)).Count);
        //    Assert.Equal(2, (await manager.GetRolesAsync(userByName)).Count);
    }

    [Fact]
    public async Task LoadFromDbFindByLoginTest()
    {
        var db = CreateContext();
        var user = CreateTestUser();
        await LazyLoadTestSetup(db, user);

        var manager = CreateManager(db);
        var userByLogin = await manager.FindByLoginAsync("provider", user.Id.ToString());
        Assert.Equal(2, (await manager.GetClaimsAsync(userByLogin)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByLogin)).Count);
        //  Assert.Equal(2, (await manager.GetRolesAsync(userByLogin)).Count);
    }

    [Fact]
    public async Task LoadFromDbFindByEmailTest()
    {
        string Connection = $"DataSource=D{Guid.NewGuid()}.db";
        var db = CreateContext(Connection);
        var user = CreateTestUser();
        user.Email = "fooz@fizzy.pop";
        await LazyLoadTestSetup(db, user);

        db = CreateContext(Connection);
        var manager = CreateManager(db);
        var userByEmail = await manager.FindByEmailAsync(user.Email);
        Assert.Equal(2, (await manager.GetClaimsAsync(userByEmail)).Count);
        Assert.Equal(1, (await manager.GetLoginsAsync(userByEmail)).Count);
        //     Assert.Equal(2, (await manager.GetRolesAsync(userByEmail)).Count);
    }
}
