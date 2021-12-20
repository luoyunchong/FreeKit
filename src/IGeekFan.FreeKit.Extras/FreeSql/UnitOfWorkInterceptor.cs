using Castle.DynamicProxy;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public class UnitOfWorkInterceptor : IInterceptor
{
    private readonly UnitOfWorkAsyncInterceptor asyncInterceptor;

    public UnitOfWorkInterceptor(UnitOfWorkAsyncInterceptor interceptor)
    {
        asyncInterceptor = interceptor;
    }

    public void Intercept(IInvocation invocation)
    {
        asyncInterceptor.ToInterceptor().Intercept(invocation);
    }
}
