using FreeSql.Internal;
using FreeSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test
{
    public class DbUtil
    {
        public static ServiceCollection Create<T>(string connectString = "Data Source=:memory:;") where T : DbContext
        {
            var services = new ServiceCollection();
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, connectString)
                .UseNoneCommandParameter(true)
                .UseGenerateCommandParameterWithLambda(false)
                .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                .UseMonitorCommand(cmd =>
                {
                    Trace.WriteLine(cmd.CommandText + ";");
                })
                .Build();

            services.AddFreeDbContext<T>(options =>
                options.UseFreeSql(fsql).UseOptions(
                    new DbContextOptions()
                    {
                        EnableCascadeSave = true
                    })
            );
            services.AddSingleton(fsql);

            services.AddFreeRepository();
            services.AddScoped<UnitOfWorkManager>();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());


            services.AddLogging();

            services.AddHttpContextAccessor();

            return services;
        }
    }

}
