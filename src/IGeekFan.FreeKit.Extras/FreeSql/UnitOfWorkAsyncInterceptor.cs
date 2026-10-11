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
    private readonly IOptionsMonitor<UnitOfWorkDefaultOptions> _unitOfWorkOptions;
    private readonly IMediator _mediator;

    public UnitOfWorkAsyncInterceptor(
        UnitOfWorkManager unitOfWorkManager,
        IOptionsMonitor<UnitOfWorkDefaultOptions> unitOfWorkOptions,
        IMediator mediator)
    {
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _unitOfWorkOptions = unitOfWorkOptions ?? throw new ArgumentNullException(nameof(unitOfWorkOptions));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// 当只配置特性标签，但未指定任意属性值时，默认根据 UnitOfWorkDefaultOptions。
    /// </summary>
    /// <param name="invocation"></param>
    /// <returns>新的工作单元；如果当前方法没有事务特性则返回 null。</returns>
    private IUnitOfWork? TryBegin(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        TransactionalAttribute? attribute = method.GetUnitOfWorkAttributeOrNull();
        if (attribute is null || attribute.IsDisabled)
        {
            return null;
        }

        UnitOfWorkDefaultOptions options = _unitOfWorkOptions.CurrentValue;
        return _unitOfWorkManager.Begin(
            attribute.Propagation ?? options.Propagation ?? Propagation.Required,
            attribute.IsolationLevel ?? options.IsolationLevel);
    }
    #endregion

    #region 拦截同步执行的方法
    /// <summary>
    /// 拦截同步执行的方法
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        IUnitOfWork? unitOfWork = TryBegin(invocation);
        if (unitOfWork is not null)
        {
            try
            {
                invocation.Proceed();
                DispatchDomainEventsAsync(unitOfWork).GetAwaiter().GetResult();
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
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
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        IUnitOfWork? unitOfWork = TryBegin(invocation);
        if (unitOfWork is not null)
        {
            try
            {
                invocation.Proceed();
                //处理Task返回一个null值的情况会导致空指针
                if (invocation.ReturnValue != null)
                {
                    await (Task)invocation.ReturnValue;
                }

                await DispatchDomainEventsAsync(unitOfWork);
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        else
        {
            invocation.Proceed();
            if (invocation.ReturnValue != null)
            {
                await (Task)invocation.ReturnValue;
            }
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
        IUnitOfWork? unitOfWork = TryBegin(invocation);
        if (unitOfWork is not null)
        {
            try
            {
                invocation.Proceed();
                TResult result = await (Task<TResult>)invocation.ReturnValue!;
                await DispatchDomainEventsAsync(unitOfWork);
                unitOfWork.Commit();
                return result;
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        invocation.Proceed();
        return await (Task<TResult>)invocation.ReturnValue!;
    }
    #endregion

    private const int Max_Deep = 10;

    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <returns></returns>
    private async Task DispatchDomainEventsAsync(IUnitOfWork unitOfWork, int deep = 0)
    {
        if (!_unitOfWorkOptions.CurrentValue.PublishDomainEvent)
        {
            return;
        }

        if (deep > Max_Deep)
        {
            throw new RecursionOverflowException(Max_Deep, "领域事件发布超过最大递归深度");
        }

        List<IDomainEventBase> domainEntities = unitOfWork.EntityChangeReport.Report
         .Select(r => r.Object as IDomainEventBase)
         .Where(r => r is not null && r.DomainEvents.Any())
         .Select(r => r!)
         .ToList();

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
                await _mediator.Publish(domainEvent);
            }
        }

        await DispatchDomainEventsAsync(unitOfWork, deep + 1);
    }
}
