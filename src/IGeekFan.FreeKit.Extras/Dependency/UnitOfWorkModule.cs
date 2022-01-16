using Autofac;
using Autofac.Extras.DynamicProxy;
using IGeekFan.FreeKit.Extras.FreeSql;
using System.Reflection;

namespace IGeekFan.FreeKit.Extras.Dependency;

public class UnitOfWorkModule : Autofac.Module
{
    private readonly Assembly[] _currentAssemblies;

    public UnitOfWorkModule(params Assembly[] currentAssemblies)
    {
        _currentAssemblies = currentAssemblies;
    }
    public UnitOfWorkModule(params Type[] types)
    {
        if (types != null && types.Length != 0)
        {
            _currentAssemblies = new Assembly[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                _currentAssemblies[i] = types[i].Assembly;
            }
        }
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UnitOfWorkInterceptor>();
        builder.RegisterType<UnitOfWorkAsyncInterceptor>();

        List<Type> interceptorServiceTypes = new List<Type>()
        {
            typeof(UnitOfWorkInterceptor),
        };

        string[] notIncludes = new string[]
        {

        };

        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(a => a.Name.EndsWith("Service") && !notIncludes.Where(r => r == a.Name).Any() && !a.IsAbstract && !a.IsInterface && a.IsPublic)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired()// 属性注入
            .InterceptedBy(interceptorServiceTypes.ToArray())
            .EnableInterfaceInterceptors();


    }
}