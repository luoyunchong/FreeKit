using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGeekFan.FreeKit.Modularity;

/// <summary>
/// 在注册服务前解析并校验 Host 的模块组合，使注册与程序集扫描使用同一份快照。
/// </summary>
public sealed class ModuleComposition
{
    private readonly KeyValuePair<string, Type>[] _registrations;

    private ModuleComposition(
        Dictionary<string, Type> effectiveModules,
        List<KeyValuePair<string, Type>> overriddenCoreModules,
        IEnumerable<Assembly>? extraAssemblies)
    {
        _registrations = overriddenCoreModules.Concat(effectiveModules).ToArray();
        Assemblies = effectiveModules.Values.Select(type => type.Assembly)
            .Concat(extraAssemblies ?? [])
            .Concat(overriddenCoreModules.Select(module => module.Value.Assembly))
            .Distinct()
            .ToArray();
        SwaggerProjectNames = effectiveModules.Values
            .Select(type => type.Assembly.GetName().Name)
            .OfType<string>()
            .Distinct()
            .ToArray();
    }

    public IReadOnlyList<Assembly> Assemblies { get; }

    public IReadOnlyList<string> SwaggerProjectNames { get; }

    public static ModuleComposition Create(
        IDictionary<string, Type> moduleTypeMap,
        IDictionary<string, Type>? moduleTypeOverrides = null,
        IEnumerable<Assembly>? extraAssemblies = null)
    {
        ArgumentNullException.ThrowIfNull(moduleTypeMap);

        var effectiveModules = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        foreach (var (moduleName, startupType) in moduleTypeMap)
        {
            ValidateModule(moduleName, startupType, nameof(moduleTypeMap));
            if (!effectiveModules.TryAdd(moduleName, startupType))
                throw new ArgumentException($"Duplicate module '{moduleName}'.", nameof(moduleTypeMap));
        }

        var overriddenCoreModules = new List<KeyValuePair<string, Type>>();
        var overriddenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (moduleName, overrideType) in moduleTypeOverrides ?? new Dictionary<string, Type>())
        {
            ValidateModule(moduleName, overrideType, nameof(moduleTypeOverrides));
            if (!overriddenNames.Add(moduleName))
                throw new ArgumentException($"Duplicate module override '{moduleName}'.", nameof(moduleTypeOverrides));

            if (!effectiveModules.TryGetValue(moduleName, out var coreType))
                throw new ArgumentException($"Cannot override unknown module '{moduleName}'.", nameof(moduleTypeOverrides));

            if (coreType != overrideType)
                overriddenCoreModules.Add(new KeyValuePair<string, Type>(moduleName, coreType));

            effectiveModules[moduleName] = overrideType;
        }

        return new ModuleComposition(effectiveModules, overriddenCoreModules, extraAssemblies);
    }

    public void RegisterServices(
        IServiceCollection services, IConfiguration configuration, bool includeControllers = true)
    {
        foreach (var (moduleName, startupType) in _registrations)
        {
            if (includeControllers)
                services.AddModule(startupType, moduleName, configuration);
            else
                services.AddModuleServices(startupType, moduleName, configuration);
        }
    }

    private static void ValidateModule(string moduleName, Type? startupType, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
            throw new ArgumentException("Module names cannot be empty.", parameterName);

        if (startupType == null || !startupType.IsClass || startupType.IsAbstract ||
            startupType.ContainsGenericParameters || !typeof(IModuleStartup).IsAssignableFrom(startupType) ||
            startupType.GetConstructor(Type.EmptyTypes) == null)
            throw new ArgumentException(
                $"Module '{moduleName}' must use a concrete {nameof(IModuleStartup)} with a public parameterless constructor.",
                parameterName);
    }
}
