
using FreeSql;
using FreeSql.Extensions.EfCoreFluentApi;
using System.Reflection;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public static class CodeFirstExtensions
{
    public static ICodeFirst ApplyConfiguration<TEntity>(this ICodeFirst codeFirst, IEntityTypeConfiguration<TEntity> configuration) where TEntity : class
    {
        return codeFirst.Entity<TEntity>(eb =>
        {
            configuration.Configure(eb);
        });
    }

    public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType)
    {
        var query = from type in assembly.GetTypes()
                    where !type.IsGenericType && !type.IsNested
                    from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                    where method.GetParameters()[0].ParameterType == extendedType
                    select method;
        return query;
    }

    public static void ApplyConfigurationsFromAssembly(this ICodeFirst codeFirst, Assembly assembly, Func<Type, bool>? predicate = null)
    {
        IEnumerable<TypeInfo> typeInfos = assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

        MethodInfo? methodInfo = typeof(FreeSqlDbContextExtensions).Assembly.GetExtensionMethods(typeof(ICodeFirst))
            .Single((e) => e.Name == "Entity" && e.ContainsGenericParameters);

        if (methodInfo == null) return;

        foreach (TypeInfo constructibleType in typeInfos)
        {
            if (constructibleType.GetConstructor(Type.EmptyTypes) == null || predicate != null && !predicate(constructibleType))
            {
                continue;
            }

            foreach (var @interface in constructibleType.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                {
                    var type = @interface.GetGenericArguments().First();
                    var efFluentType = typeof(EfCoreTableFluent<>).MakeGenericType(type);
                    var actionType = typeof(Action<>).MakeGenericType(efFluentType);

                    //1.需要实体和Configuration配置
                    //codeFirst.Entity<Todo>(eb =>
                    //{
                    //    new TodoConfiguration().Configure(eb);
                    //});

                    //2.需要实体
                    //Action<EfCoreTableFluent<Todo>> x = new Action<EfCoreTableFluent<Todo>>(e =>
                    //{
                    //    object? o = Activator.CreateInstance(constructibleType);
                    //    constructibleType.GetMethod("ApplyConfiguration")?.Invoke(o, new object[1] { e });
                    //});
                    //codeFirst.Entity<Todo>(x);

                    //3.实现动态调用泛型委托
                    DelegateBuilder delegateBuilder = new DelegateBuilder(constructibleType);
                    MethodInfo? configureMethodInfo = delegateBuilder.GetType().GetMethod("ApplyConfiguration")?.MakeGenericMethod(type);
                    if (configureMethodInfo == null) continue;
                    Delegate @delegate = Delegate.CreateDelegate(actionType, delegateBuilder, configureMethodInfo);

                    methodInfo.MakeGenericMethod(type).Invoke(null, new object[2]
                    {
                        codeFirst,
                        @delegate
                    });

                }
            }
        }
    }


    class DelegateBuilder
    {
        private readonly Type type;

        public DelegateBuilder(Type type)
        {
            this.type = type;
        }

        public void ApplyConfiguration<T>(EfCoreTableFluent<T> ex)
        {
            object? o = Activator.CreateInstance(type);
            type.GetMethod("Configure")?.Invoke(o, new object[1] { ex });
        }
    }

}