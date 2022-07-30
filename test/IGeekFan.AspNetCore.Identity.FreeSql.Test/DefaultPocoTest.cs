using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityResultAssert = IGeekFan.AspNetCore.Identity.FreeSql.Test.Specification.Tests.IdentityResultAssert;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test
{
    public class DefaultPocoTest : IClassFixture<ScratchDatabaseFixture>
    {
        private readonly ApplicationBuilder _builder;
        
        public DefaultPocoTest(ScratchDatabaseFixture fixture)
        {
            var services = DbUtil.Create<IdentityDbContext>();
            
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddFreeSqlStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();

            _builder = new ApplicationBuilder(provider);

            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbcontext = scoped.ServiceProvider.GetRequiredService<IdentityDbContext>();
                dbcontext.Orm.CodeFirst.IsAutoSyncStructure = true;
                dbcontext.Orm.CodeFirst.SyncStructure(typeof(IdentityUser), typeof(IdentityRole));
            }
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
    }

}
