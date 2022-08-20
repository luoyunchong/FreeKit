// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 
/// </summary>
public class ReflexHelper
{
    /// <summary>
    /// 扫描type所在程序集，反射得到类上有特性标签为TableAttribute 的所有类
    /// </summary>
    /// <returns></returns>
    public static Type[] GetTypesByTableAttribute(Type type)
    {
        var assembly = Assembly.GetAssembly(type);
        if (assembly == null) return Array.Empty<Type>();

        List<Type> tableAssembies = assembly.GetExportedTypes()
            .Where(t => t.GetCustomAttributes<TableAttribute>(false).FirstOrDefault()?.DisableSyncStructure == false)
            .ToList();
        return tableAssembies.ToArray();
    }

    /// <summary>
    /// 扫描type所在程序集，反射得到某命名空间下的所有类
    /// </summary>
    /// <param name="assemblyType"></param>
    /// <param name="entitiesFullName"></param>
    /// <returns></returns>
    public static Type[] GetTypesByNameSpace(Type assemblyType, List<string> entitiesFullName)
    {
        Assembly? assembly = Assembly.GetAssembly(assemblyType);
        if (assembly == null) return Array.Empty<Type>();

        List<Type> tableAssembies = assembly
            .GetExportedTypes()
            .Where(t =>
                entitiesFullName.Any(u => t.FullName != null && t.FullName.StartsWith(u) && t.IsClass)
            )
            .ToList();

        return tableAssembies.ToArray();
    }
}
