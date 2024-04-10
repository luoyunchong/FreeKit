// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Reflection;
using IGeekFan.FreeKit.Extras.Extensions;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.Extensions.Configuration;

namespace FreeSql
{
    /// <summary>
    /// FreeSql相关的扩展类
    /// </summary>
    public static class FreeSqlExtension
    {
        /// <summary>
        /// 根据配置文件配置
        /// </summary>
        /// <param name="this"></param>
        /// <param name="configuration"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static FreeSqlBuilder UseConnectionString(this FreeSqlBuilder @this, IConfiguration configuration, string prefix = "ConnectionStrings")
        {
            IConfigurationSection dbTypeCode = configuration.GetSection($"{prefix}:DefaultDB");
            IConfigurationSection providerTypeSection = configuration.GetSection($"{prefix}:ProviderType");
            var providerType = providerTypeSection.Value.IsNotNullOrWhiteSpace()
                ? Type.GetType(providerTypeSection.Value)
                : null;

            if (Enum.TryParse(dbTypeCode.Value, out DataType dataType))
            {
                if (!Enum.IsDefined(typeof(DataType), dataType))
                {
                    Trace.WriteLine($"数据库配置{{prefixs:DefaultDB:{dataType}无效");
                }

                IConfigurationSection connectionStringSection =
                    configuration.GetSection($"{prefix}:{dataType}");
                @this.UseConnectionString(dataType, connectionStringSection.Value, providerType);
            }
            else
            {
                Trace.WriteLine($"数据库配置{prefix}:DefaultDB:{dbTypeCode.Value}无效");
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
            FieldInfo? fieldInfo =
                type.GetField("_masterConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
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
            List<string> tableNames = new List<string>();
            for (int i = 0; i < count; i++)
            {
                tableNames.AddIfNotContains($"{tableName}_{i}");
            }

            @this.AsTable(tableNames.ToArray());
            return @this;
        }

        public static ISelect<T> AsTable<T>(this ISelect<T> @this, params string[] tableNames) where T : class
        {
            tableNames?.ToList().ForEach(tableName =>
            {
                @this.AsTable((type, oldname) =>
                {
                    if (type == typeof(T))
                    {
                        return tableName.Replace("{oldname}", oldname);
                    }

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
                switch (e.Column.CsName)
                {
                    case "CreateUserId":
                        T? userId = user?.FindUserId<T>();
                        if (userId.HasValue && e.Value != null) e.Value = userId;
                        break;
                    case "CreateUserName":
                        string? userName = user?.UserName;
                        if (userName.IsNotNullOrWhiteSpace() && e.Value != null) e.Value = userName;
                        break;
                    case "CreateTime":
                        e.Value = DateTime.Now;
                        break;
                    case "TenantId":
                        var tenantId = user?.TenantId;
                        if (tenantId.HasValue && e.Value != null) e.Value = tenantId;
                        break;
                }
            }
            else if (e.AuditValueType == AuditValueType.Update)
            {
                switch (e.Column.CsName)
                {
                    case "UpdateUserId":
                        T? userId = user?.FindUserId<T>();
                        if (userId.HasValue && e.Value != null) e.Value = userId;
                        break;
                    case "UpdateUserName":
                        string? userName = user?.UserName;
                        if (userName.IsNotNullOrWhiteSpace() && e.Value != null) e.Value = userName;
                        break;
                    case "UpdateTime":
                        e.Value = DateTime.Now;
                        break;
                }
            }
        }
    }
}