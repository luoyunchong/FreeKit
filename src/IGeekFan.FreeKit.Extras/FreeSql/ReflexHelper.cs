using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 
/// </summary>
public class ReflexHelper
{
    /// <summary>
    /// 扫描 IEntity类所在程序集，反射得到类上有特性标签为TableAttribute 的所有类
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
                if (attribute is TableAttribute tableAttribute)
                {
                    if (tableAttribute.DisableSyncStructure == false)
                    {
                        tableAssembies.Add(itemType);
                    }
                }
            }
        };
        return tableAssembies.ToArray();
    }
}
