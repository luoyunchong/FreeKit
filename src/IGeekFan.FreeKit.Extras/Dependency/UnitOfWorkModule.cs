using Autofac;
using Autofac.Extras.DynamicProxy;
using IGeekFan.FreeKit.Extras.FreeSql;
using System.Reflection;

namespace IGeekFan.FreeKit.Extras.Dependency;

public class UnitOfWorkModule : Autofac.Module
{
    private readonly Assembly[] assemblies;

    public UnitOfWorkModule(Assembly[] assemblies)
    {
        this.assemblies = assemblies;
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
        //Assembly servicesDllFile = Assembly.Load("LinCms.Application");
        builder.RegisterAssemblyTypes(assemblies)
            .Where(a => a.Name.EndsWith("Service") && !notIncludes.Where(r => r == a.Name).Any() && !a.IsAbstract && !a.IsInterface && a.IsPublic)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired()// 属性注入
            .InterceptedBy(interceptorServiceTypes.ToArray())
            .EnableInterfaceInterceptors();


    }
}