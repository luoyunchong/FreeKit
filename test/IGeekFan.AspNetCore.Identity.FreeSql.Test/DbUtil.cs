using FreeSql.Internal;
using FreeSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Sqlite;
using FreeSql.DatabaseModel;

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

        public static ServiceCollection CreateCustomPocoContext<T>(string connectString = "Data Source=:memory:;") where T : DbContext
        {
            var services = new ServiceCollection();
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, connectString)
                .UseNoneCommandParameter(true)
                .UseGenerateCommandParameterWithLambda(false)
                .UseNameConvert(NameConvertType.None)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                .UseMonitorCommand(cmd =>
                {
                    Trace.WriteLine(cmd.CommandText + ";");
                })
                .Build();

            services.AddFreeDbContext<T>(options => options.UseFreeSql(fsql).UseOptions(
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


        public static bool VerifyMaxLength(DbContext context, string table, int maxLength, params string[] columns)
        {
            var count = 0;

            DbTableInfo dbTableInfo = context.Orm.DbFirst.GetTableByName(table);

            foreach (var a in dbTableInfo.Columns)
            {
                if (columns.Contains(a.Name))
                {
                    if (a.MaxLength != maxLength)
                    {
                        return false;
                    }
                    count++;
                }
            }
            return count == columns.Length;
        }

        public static bool VerifyColumns(SqliteConnection conn, string table, params string[] columns)
        {
            var count = 0;
            using (var command = new SqliteCommand("SELECT \"name\" FROM pragma_table_info(@table)", conn))
            {
                command.Parameters.Add(new SqliteParameter("table", table));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        if (!columns.Contains(reader.GetString(0)))
                        {
                            return false;
                        }
                    }
                    return count == columns.Length;
                }
            }
        }

        public static void VerifyIndex(SqliteConnection conn, string table, string index, bool isUnique = false)
        {
            using (var command =
                new SqliteCommand(
                    "SELECT COUNT(*) FROM pragma_index_list(@table) WHERE \"name\" = @index AND \"unique\" = @unique", conn))
            {
                command.Parameters.Add(new SqliteParameter("index", index));
                command.Parameters.Add(new SqliteParameter("table", table));
                command.Parameters.Add(new SqliteParameter("unique", isUnique));
                using (var reader = command.ExecuteReader())
                {
                    Assert.True(reader.Read());
                    Assert.True(reader.GetInt32(0) > 0);
                }
            }
        }
    }


}
