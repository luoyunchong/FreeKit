// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using IGeekFan.FreeKit.Extras.FreeSql;

namespace IGeekFan.FreeKit.Extras.Dependency;

public class UnitOfWorkModule : Autofac.Module
{
    private readonly Assembly[] _currentAssemblies;

    readonly List<Type> _interceptorServiceTypes = new List<Type>()
    {
        typeof(UnitOfWorkInterceptor)
    };

    public UnitOfWorkModule(Assembly[] currentAssemblies)
    {
        _currentAssemblies = currentAssemblies;
    }

    public UnitOfWorkModule(Assembly[] currentAssemblies,List<Type>? interceptorServiceTypes)
    {
        _currentAssemblies = currentAssemblies;
        if (interceptorServiceTypes != null && interceptorServiceTypes.Count != 0)
        {
            _interceptorServiceTypes.AddRange(interceptorServiceTypes);
        }
    }

    public UnitOfWorkModule(Type[]? typeAssemblies, List<Type>? interceptorServiceTypes)
    {
        if (typeAssemblies != null && typeAssemblies.Length != 0)
        {
            _currentAssemblies = new Assembly[typeAssemblies.Length];
            for (int i = 0; i < typeAssemblies.Length; i++)
            {
                _currentAssemblies[i] = typeAssemblies[i].Assembly;
            }
        }

        if (interceptorServiceTypes != null && interceptorServiceTypes.Count != 0)
        {
            _interceptorServiceTypes.AddRange(interceptorServiceTypes);
        }
    }

    protected override void Load(ContainerBuilder builder)
    {
        //Instance Per Dependency是默认的模式
        builder.RegisterType<UnitOfWorkInterceptor>();
        builder.RegisterType<UnitOfWorkAsyncInterceptor>();

        bool Predicate(Type a) => !a.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && a.Name.EndsWith("Service") && !a.IsAbstract && !a.IsInterface && a.IsPublic;

        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(Predicate)
            .AsImplementedInterfaces()//注册的类型，以接口的方式注册
            .InstancePerLifetimeScope()
            .PropertiesAutowired()// 属性注入
            .InterceptedBy(_interceptorServiceTypes.ToArray())
            .EnableInterfaceInterceptors();

        builder.RegisterAssemblyTypes(_currentAssemblies)
            .Where(Predicate)
            .AsSelf()
            .InstancePerLifetimeScope()
            .PropertiesAutowired()// 属性注入
            .InterceptedBy(_interceptorServiceTypes.ToArray())
            .EnableClassInterceptors();

    }
}