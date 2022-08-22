// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Reflection;
using FreeSql;
using FreeSql.Aop;
using IGeekFan.FreeKit.Extras.Security;
using Microsoft.Extensions.Configuration;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public static class FreeSqlExtension
{
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

}
