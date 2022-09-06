// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Reflection;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.Extensions.Configuration;

namespace FreeSql
{
    public static class FreeSqlExtension
    {
        /// <summary>
        /// 根据配置文件配置
        /// </summary>
        /// <param name="this"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static FreeSqlBuilder UseConnectionString(this FreeSqlBuilder @this, IConfiguration configuration)
        {
            IConfigurationSection dbTypeCode = configuration.GetSection("ConnectionStrings:DefaultDB");
            if (Enum.TryParse(dbTypeCode.Value, out DataType dataType))
            {
                if (!Enum.IsDefined(typeof(DataType), dataType))
                {
                    Trace.WriteLine($"数据库配置ConnectionStrings:DefaultDB:{dataType}无效");
                }

                IConfigurationSection configurationSection = configuration.GetSection($"ConnectionStrings:{dataType}");
                @this.UseConnectionString(dataType, configurationSection.Value);
            }
            else
            {
                Trace.WriteLine($"数据库配置ConnectionStrings:DefaultDB:{dbTypeCode.Value}无效");
            }

            return @this;
        }

        /// <summary>
        /// 获取UseConnectionString中的数据库连接串
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetConnectionString(this FreeSqlBuilder @this)
        {
            Type type = @this.GetType();
            FieldInfo? fieldInfo = type.GetField("_masterConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo is null)
            {
                throw new ArgumentException("_masterConnectionString is null");
            }
            string? connectionString = fieldInfo.GetValue(@this)?.ToString();
            if (connectionString is null)
            {
                throw new ArgumentException("_masterConnectionString is null");
            }
            return connectionString;
        }

        public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
        {
            if (!@this.Contains(value))
            {
                @this.Add(value);
                return true;
            }

            return false;
        }
        public static ISelect<T> AsTable<T>(this ISelect<T> @this, string tableName, int count) where T : class
        {
            string[] tableNames = Array.Empty<string>();
            for (int i = 0; i < count; i++)
            {
                tableNames.AddIfNotContains($"{tableName}_{i}");
            }

            @this.AsTable(tableNames);
            return @this;
        }

        public static ISelect<T> AsTable<T>(this ISelect<T> @this, params string[] tableNames) where T : class
        {
            tableNames?.ToList().ForEach(tableName =>
            {
                @this.AsTable((type, oldname) =>
                {
                    if (type == typeof(T)) return tableName;
                    return null;
                });
            });
            return @this;
        }

        /// <summary>
        /// 不跟踪查询的实体数据（在不需要更新其数据时使用），可提升查询性能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static ISelect<T> AsNoTracking<T>(this ISelect<T> select) where T : class
        {
            return select.NoTracking();
        }
    }
}

namespace FreeSql.Aop
{
    public static class AuditValueEventArgsExtension
    {
        /// <summary>
        /// 审计实体，创建、更新将自动审计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="user"></param>
        public static void AuditValue<T>(this AuditValueEventArgs e, ICurrentUser? user) where T : struct
        {
            if (e.AuditValueType == AuditValueType.Insert)
            {
                e.Value = e.Column.CsName switch
                {
                    "CreateUserId" => user?.FindUserId<T>(),
                    "CreateUserName" => user?.UserName,
                    "CreateTime" => DateTime.Now,
                    "TenantId" => user?.TenantId,
                    _ => e.Value
                };
            }
            else if (e.AuditValueType == AuditValueType.Update)
            {
                e.Value = e.Column.CsName switch
                {
                    "UpdateUserId" => user?.FindUserId<T>(),
                    "UpdateUserName" => user?.UserName,
                    "UpdateTime" => DateTime.Now,
                    _ => e.Value
                };
            }
        }
    }
}