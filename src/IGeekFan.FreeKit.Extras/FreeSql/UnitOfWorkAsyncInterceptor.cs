// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Castle.DynamicProxy;
using FreeSql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public static class MethodInfoExtensions
{
    /// <summary>
    /// 判断方法上或类上是否有TransactionalAttribute特性标签 
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    public static TransactionalAttribute? GetUnitOfWorkAttributeOrNull(this MethodInfo methodInfo)
    {
        var attrs = methodInfo.GetCustomAttributes(true).OfType<TransactionalAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        if (methodInfo.DeclaringType == null) return null;
        attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<TransactionalAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        return null;
    }
}

/// <summary>
/// 异步事务方法拦截
/// </summary>
public class UnitOfWorkAsyncInterceptor : IAsyncInterceptor
{
    private readonly UnitOfWorkManager _unitOfWorkManager;
    private readonly ILogger<UnitOfWorkAsyncInterceptor> _logger;
    private IUnitOfWork _unitOfWork;
    private readonly UnitOfWorkDefualtOptions _unitOfWorkDefualtOptions;

    public UnitOfWorkAsyncInterceptor(UnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkAsyncInterceptor> logger, IOptionsMonitor<UnitOfWorkDefualtOptions> unitOfWorkDefualtOptions)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
        _unitOfWorkDefualtOptions = unitOfWorkDefualtOptions.CurrentValue;
    }


    /// <summary>
    /// 当只配置特性标签，但未指定任意属性值时，默认根据UnitOfWorkDefualtOptions
    /// </summary>
    /// <param name="invocation"></param>
    /// <returns></returns>
    private bool TryBegin(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        var transaction = method.GetUnitOfWorkAttributeOrNull();
        if (transaction != null)
        {
            if (transaction.IsDisabled) return false;
            transaction.IsolationLevel ??= _unitOfWorkDefualtOptions.IsolationLevel;
            transaction.Propagation ??= _unitOfWorkDefualtOptions.Propagation;
            _unitOfWork = _unitOfWorkManager.Begin(transaction.Propagation ?? Propagation.Required, transaction.IsolationLevel);
            return true;
        }
        return false;
    }

    #region 拦截同步执行的方法
    /// <summary>
    /// 拦截同步执行的方法
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            try
            {
                invocation.Proceed();
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        else
        {
            invocation.Proceed();
        }
    }
    #endregion

    #region 拦截返回结果为Task的方法
    /// <summary>
    /// 拦截返回结果为Task的方法
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }
        else
        {
            invocation.Proceed();
        }
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
            //处理Task返回一个null值的情况会导致空指针
            if (invocation.ReturnValue != null)
            {
                await (Task)invocation.ReturnValue;
            }
            _unitOfWork.Commit();
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }

    }

    #endregion

    #region 拦截返回结果为Task<TResult>的方法
    /// <summary>
    /// 拦截返回结果为Task<TResult>的方法
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }
    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        TResult result;
        if (TryBegin(invocation))
        {
            try
            {
                invocation.Proceed();
                Task<TResult> task = (Task<TResult>)invocation.ReturnValue;
                result = await task;
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        else
        {
            invocation.Proceed();
            Task<TResult> task = (Task<TResult>)invocation.ReturnValue;
            result = await task;
        }
        return result;
    }
    #endregion
}