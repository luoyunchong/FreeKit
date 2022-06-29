using FreeSql;
using FreeSql.DataAnnotations;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;


public class CustomPocoTest : IClassFixture<ScratchDatabaseFixture>
{
    public class User<TKey> where TKey : IEquatable<TKey>
    {
        [Column(IsPrimary = true)]
        public TKey Id { get; set; }
        public string UserName { get; set; }
    }

    public class CustomDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
    {
        public CustomDbContext(IFreeSql fsql, DbContextOptions options) : base(fsql, options) { }
        public DbSet<User<TKey>> Users { get; set; }
    }
    
    protected readonly ScratchDatabaseFixture _fixture;

    public CustomPocoTest(ScratchDatabaseFixture fixture)
    {
        _fixture = fixture;
    }


    public CustomDbContext<T> CreateContext<T>() where T : IEquatable<T>
    {
        string dbname = $"DataSource=D{Guid.NewGuid()}.db";
        var services = DbUtil.CreateCustomPocoContext<CustomDbContext<T>>(dbname??_fixture.Connection);
        var db = services.BuildServiceProvider().GetRequiredService<CustomDbContext<T>>();
        return db;
    }

    [Fact]
    public async Task CanUpdateNameGuid()
    {
        using (var db = CreateContext<Guid>())
        {
            var oldName = Guid.NewGuid().ToString();
            var user = new User<Guid> { UserName = oldName, Id = Guid.NewGuid() };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var newName = Guid.NewGuid().ToString();
            user.UserName = newName;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            Assert.Null(db.Users.Select.Where(u => u.UserName == oldName).First());
            MyAssert.Equal(user, db.Users.Select.Where(u => u.UserName == newName).First());

        }
    }

    [Fact]
    public async Task CanUpdateNameString()
    {
        using (var db = CreateContext<string>())
        {
            var oldName = Guid.NewGuid().ToString();
            var user = new User<string> { UserName = oldName, Id = Guid.NewGuid().ToString() };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var newName = Guid.NewGuid().ToString();
            user.UserName = newName;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            Assert.Null(db.Users.Select.Where(u => u.UserName == oldName).First());
            MyAssert.Equal(user, db.Users.Select.Where(u => u.UserName == newName).First());

        }
    }

    [Fact]
    public async Task CanCreateUserInt()
    {
        using (var db = CreateContext<int>())
        {
            var user = new User<int>();
            db.Users.Add(user);
            await db.SaveChangesAsync();
            user.UserName = "Boo";
            db.Users.Update(user);
            await db.SaveChangesAsync();
            var fetch = db.Users.Select.Where(u => u.UserName == "Boo").First();
            MyAssert.Equal(user, fetch);

        }
    }

    [Fact]
    public async Task CanCreateUserIntViaSet()
    {
        using (var db = CreateContext<int>())
        {

            var user = new User<int>();
            var users = db.Set<User<int>>();
            users.Add(user);
            await db.SaveChangesAsync();
            user.UserName = "Boo";
            db.Users.Update(user);
            await db.SaveChangesAsync();
            var fetch = users.Select.Where(u => u.UserName == "Boo").First();
            MyAssert.Equal(user, fetch);

        }
    }

    [Fact]
    public async Task CanUpdateNameInt()
    {
        using (var db = CreateContext<int>())
        {
            var oldName = Guid.NewGuid().ToString();
            var user = new User<int> { UserName = oldName };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var newName = Guid.NewGuid().ToString();
            user.UserName = newName;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            Assert.Null(db.Users.Select.Where(u => u.UserName == oldName).First());
            MyAssert.Equal(user, db.Users.Select.Where(u => u.UserName == newName).First());

        }
    }

    [Fact]
    public async Task CanUpdateNameIntWithSet()
    {
        using (var db = CreateContext<int>())
        {
            var oldName = Guid.NewGuid().ToString();
            var user = new User<int> { UserName = oldName };
            db.Set<User<int>>().Add(user);
            await db.SaveChangesAsync();
            var newName = Guid.NewGuid().ToString();
            user.UserName = newName;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            Assert.Null(db.Users.Select.Where(u => u.UserName == oldName).First());
            MyAssert.Equal(user, db.Users.Select.Where(u => u.UserName == newName).First());

        }
    }
}
