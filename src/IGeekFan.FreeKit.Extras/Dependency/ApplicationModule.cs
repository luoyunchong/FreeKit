using Autofac;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace IGeekFan.FreeKit.Extras.Dependency;

public class FreeKitModule : Autofac.Module
{
    Assembly[] _currentAssemblies;

    public FreeKitModule(Assembly[] currentAssemblies)
    {
        _currentAssemblies = currentAssemblies;
    }

    protected override void Load(ContainerBuilder builder)
    {
        if (_currentAssemblies == null)
        {
            _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName == "IGeekFan.FreeKit.Extras").ToArray();
        }
        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        //每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
        Type transientDependency = typeof(ITransientDependency);
        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(t => transientDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
            .AsImplementedInterfaces().InstancePerDependency();

        //同一个Lifetime生成的对象是同一个实例
        Type scopeDependency = typeof(IScopedDependency);
        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(t => scopeDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
        Type singletonDependency = typeof(ISingletonDependency);
        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(t => singletonDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
            .AsImplementedInterfaces().SingleInstance();

    }
}
