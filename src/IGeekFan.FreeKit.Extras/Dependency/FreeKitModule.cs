using Autofac;
using System.Reflection;

namespace IGeekFan.FreeKit.Extras.Dependency;

/// <summary>
///  批量注册:只需要继承接口ITransientDependency/IScopedDependency/ISingletonDependency，即可自动加入DI中
/// </summary>
public class FreeKitModule : Autofac.Module
{
    private readonly Assembly[] _currentAssemblies;

    /// <summary>
    /// 根据程序集批量注册服务
    /// </summary>
    /// <param name="currentAssemblies">需要处理的程序集</param>
    public FreeKitModule(params Assembly[] currentAssemblies)
    {
        _currentAssemblies = currentAssemblies;
    }

    /// <summary>
    ///  根据Type批量注册其程序集下所有继承ITransientDependency/IScopedDependency/ISingletonDependency的类
    /// </summary>
    /// <param name="types"></param>
    public FreeKitModule(params Type[] types)
    {
        if (types != null && types.Length > 0)
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
        //AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName == "IGeekFan.FreeKit.Extras").ToArray();
        //typeof(FreeKitModule).Assembly
        if (_currentAssemblies == null || _currentAssemblies.Length == 0)
        {
            return;
        }
        
        //每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
        bool TransientPredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;

        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(TransientPredicate)
            .AsImplementedInterfaces().InstancePerDependency();

        //同一个Lifetime生成的对象是同一个实例
        bool ScopePredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;
        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(ScopePredicate)
            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
        bool SingletonPredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;
        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(SingletonPredicate)
            .AsImplementedInterfaces().SingleInstance();

    }
}
