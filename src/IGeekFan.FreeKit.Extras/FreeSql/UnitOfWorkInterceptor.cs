// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Castle.DynamicProxy;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public class UnitOfWorkInterceptor : IInterceptor
{
    private readonly UnitOfWorkAsyncInterceptor _asyncInterceptor;

    public UnitOfWorkInterceptor(UnitOfWorkAsyncInterceptor interceptor)
    {
        _asyncInterceptor = interceptor;
    }

    public void Intercept(IInvocation invocation)
    {
        _asyncInterceptor.ToInterceptor().Intercept(invocation);
    }
}
