using FreeSql.Internal;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test
{

    public class DefaultPocoTest
    {
        private readonly ApplicationBuilder _builder;
        public DefaultPocoTest()
        {
            string connectionString = "Data Source=hello.db";

            var services = new ServiceCollection();
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, connectionString)
                .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                .UseMonitorCommand(cmd =>
                {
                    Trace.WriteLine(cmd.CommandText + ";");
                })
                .Build();

            services.AddSingleton(fsql);
            services.AddFreeRepository();
            services.AddScoped<UnitOfWorkManager>();
            services
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .AddFreeDbContext<IdentityDbContext>(options => options
                    .UseFreeSql(fsql)
                    .UseOptions(new DbContextOptions()
                    {
                        EnableCascadeSave = true
                    }));

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddFreeSqlStores<IdentityDbContext>();

            services.AddLogging();

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
