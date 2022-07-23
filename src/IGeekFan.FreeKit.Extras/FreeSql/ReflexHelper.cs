using FreeSql.DataAnnotations;
using System.Reflection;

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
    public static Type[]? GetTypesByTableAttribute(Type type)
    {
        List<Type> tableAssembies = new List<Type>();
        Assembly? assembly = Assembly.GetAssembly(type);
        if (assembly == null) return null;
        foreach (Type itemType in assembly.GetExportedTypes())
        {
            foreach (Attribute attribute in itemType.GetCustomAttributes())
            {
                if (attribute is not TableAttribute { DisableSyncStructure: false }) continue;
                tableAssembies.Add(itemType);
            }
        };
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
        List<Type> tableAssembies = new List<Type>();
        Assembly? assembly = Assembly.GetAssembly(assemblyType);
        foreach (Type type in assembly.GetExportedTypes())
        {
            foreach (var fullname in entitiesFullName)
            {
                if (type.FullName != null && type.FullName.StartsWith(fullname) && type.IsClass && !type.IsAbstract)
                {
                    tableAssembies.Add(type);
                }
            }
        }
        return tableAssembies.ToArray();
    }
}
