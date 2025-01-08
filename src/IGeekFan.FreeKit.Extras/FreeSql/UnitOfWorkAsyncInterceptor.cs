// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Reflection;
using System.Threading;

using Castle.DynamicProxy;

using FreeSql;

using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.Domain;

using MediatR;

using Microsoft.Extensions.Options;

using static FreeSql.DbContext;

namespace IGeekFan.FreeKit.Extras.FreeSql;

#region MethodInfoExtensions 扩展获取 TransactionalAttribute特性
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
#endregion

/// <summary>
/// 异步事务方法拦截
/// </summary>
public class UnitOfWorkAsyncInterceptor : IAsyncInterceptor
{
    #region UnitOfWorkAsyncInterceptor
    private readonly UnitOfWorkManager _unitOfWorkManager;
    private IUnitOfWork _unitOfWork;
    private readonly UnitOfWorkDefaultOptions _unitOfWorkDefaultOptions;
    IMediator mediator;
    public UnitOfWorkAsyncInterceptor(UnitOfWorkManager unitOfWorkManager, IOptionsMonitor<UnitOfWorkDefaultOptions> unitOfWorkDefaultOptions, IMediator mediator)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _unitOfWorkDefaultOptions = unitOfWorkDefaultOptions.CurrentValue;
        this.mediator = mediator;
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
            transaction.IsolationLevel ??= _unitOfWorkDefaultOptions.IsolationLevel;
            transaction.Propagation ??= _unitOfWorkDefaultOptions.Propagation;
            _unitOfWork = _unitOfWorkManager.Begin(transaction.Propagation ?? Propagation.Required, transaction.IsolationLevel);
            return true;
        }
        return false;
    }
    #endregion

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
                DispatchDomainEventsAsync().GetAwaiter().GetResult();
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
            if (_unitOfWorkDefaultOptions.PublishDomainEvent)
            {
                DispatchDomainEventsAsync().GetAwaiter().GetResult();
            }
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
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            try
            {
                invocation.Proceed();
                //处理Task返回一个null值的情况会导致空指针
                if (invocation.ReturnValue != null)
                {
                    await (Task)invocation.ReturnValue;
                }


                await DispatchDomainEventsAsync();
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
            if (invocation.ReturnValue != null)
            {
                await (Task)invocation.ReturnValue;
            }
            await DispatchDomainEventsAsync();
        }
    }

    #endregion

    #region 拦截返回结果为Task<TResult>的方法
    /// <summary>
    /// 拦截返回结果为Task&lt;TResult&gt;的方法
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
                result = await (Task<TResult>)invocation.ReturnValue;
                await DispatchDomainEventsAsync();
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
            await DispatchDomainEventsAsync();
            result = await (Task<TResult>)invocation.ReturnValue;
        }
        return result;
    }
    #endregion

    private const int Max_Deep = 10;

    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <returns></returns>
    private async Task DispatchDomainEventsAsync(int deep = 0)
    {
        if (!_unitOfWorkDefaultOptions.PublishDomainEvent || _unitOfWork == null)
        {
            return;
        }

        if (deep > Max_Deep)
        {
            throw new RecursionOverflowException(Max_Deep, "领域事件发布超过最大递归深度");
        }

        var domainEntities = _unitOfWork.EntityChangeReport.Report
         .Select(r => r.Object as IDomainEventBase)
         .Where(r => r != null && r.DomainEvents.Any()).ToList();

        if (domainEntities.Count == 0)
        {
            return;
        }

        var domainEvents = domainEntities.SelectMany(r => r.GetDomainEvents())
         .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        if (domainEvents.Any())
        {
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }

        await DispatchDomainEventsAsync(deep + 1);
    }
}