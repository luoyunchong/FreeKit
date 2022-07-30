namespace IGeekFan.FreeKit.Extras.Extensions;
public static class TypeExtensions
{
    /// <summary>
    /// 判断某个类型是否继承了某个泛型接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="generic"></param>
    /// <returns></returns>
    public static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        // 遍历类型实现的所有接口，判断是否存在某个接口是泛型，且是参数中指定的原始泛型的实例。
        return type.GetInterfaces().Any(x => generic == (x.IsGenericType ? x.GetGenericTypeDefinition() : x));
    }
}