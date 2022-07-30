// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using FreeSql;
using FreeSql.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks.DependencyInjection
{
    public class FreeUtil
    {
        public static ServiceCollection GetFreeSqlServiceCollection<T>() where T : DbContext
        {
            var serviceCollection = new ServiceCollection();
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, "Data Source=:memory:;")
                .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                .UseMonitorCommand(cmd =>
                {
                    Trace.WriteLine(cmd.CommandText + ";");
                })
                .Build();

            serviceCollection.AddFreeDbContext<T>(
                options => options
                    .UseFreeSql(fsql)
                    .UseOptions(new DbContextOptions()
                    {
                        EnableCascadeSave = true
                    })
            );
            return serviceCollection;
        }
    }
}
