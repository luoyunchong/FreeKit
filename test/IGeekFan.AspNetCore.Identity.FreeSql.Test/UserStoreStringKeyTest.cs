using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;

public class StringUser : IdentityUser
{
    public StringUser()
    {
        Id = Guid.NewGuid().ToString();
        UserName = Id;
    }
}

public class StringRole : IdentityRole<string>
{
    public StringRole()
    {
        Id = Guid.NewGuid().ToString();
        Name = Id;
    }
}

public class UserStoreStringKeyTest : SqlStoreTestBase<StringUser, StringRole, string>
{
    public UserStoreStringKeyTest(ScratchDatabaseFixture fixture)
        : base(fixture)
    { }

    [Fact]
    public void AddEntityFrameworkStoresCanInferKey()
    {
        var services = DbUtil.Create<TestDbContext>();
        // This used to throw
        var builder = services.AddIdentity<StringUser, StringRole>().AddFreeSqlStores<TestDbContext>();

        var sp = services.BuildServiceProvider();
        using (var csope = sp.CreateScope())
        {
            Assert.NotNull(sp.GetRequiredService<UserManager<StringUser>>());
            Assert.NotNull(sp.GetRequiredService<RoleManager<StringRole>>());
        }
    }


    [Fact]
    public void AddFreeSqlStoresCanInferKeyWithGenericBase()
    {
        var services = DbUtil.Create<TestDbContext>();
        // This used to throw
        var builder = services.AddIdentityCore<IdentityUser<string>>().AddRoles<IdentityRole<string>>().AddFreeSqlStores<TestDbContext>();

        var sp = services.BuildServiceProvider();
        using (var csope = sp.CreateScope())
        {
            Assert.NotNull(sp.GetRequiredService<UserManager<IdentityUser<string>>>());
            Assert.NotNull(sp.GetRequiredService<RoleManager<IdentityRole<string>>>());
        }
    }

}
