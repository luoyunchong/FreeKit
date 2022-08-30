// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FreeSql;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 工作单元过滤器
/// </summary>
public class UnitOfWorkActionFilter : IAsyncActionFilter
{

    /// <summary>
    /// 尝试从方法中获取当前的工作单元配置
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private TransactionalAttribute? TryGetTransactional(ActionExecutingContext context)
    {
        ControllerActionDescriptor? descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        MethodInfo method = descriptor?.MethodInfo ?? throw new ArgumentNullException("context");
        var transaction = method.GetUnitOfWorkAttributeOrNull();
        if (transaction != null)
        {
            if (transaction.IsDisabled) return null;
            var unitOfWorkDefualtOptions = context.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<UnitOfWorkDefualtOptions>>().CurrentValue;
            transaction.IsolationLevel ??= unitOfWorkDefualtOptions.IsolationLevel;
            transaction.Propagation ??= unitOfWorkDefualtOptions.Propagation;
            return transaction;
        }
        return null;
    }

    /// <summary>
    /// 方法执行前执行UnitOfWorkManager工作单元
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!(context.ActionDescriptor is ControllerActionDescriptor))
        {
            await next();
            return;
        }

        var transaction = TryGetTransactional(context);
        if (transaction == null)
        {
            await next();
            return;
        }

        UnitOfWorkManager unitOfWorkManager = context.HttpContext.RequestServices.GetRequiredService<UnitOfWorkManager>();
        using IUnitOfWork unitOfWork = unitOfWorkManager.Begin(transaction.Propagation ?? Propagation.Required, transaction.IsolationLevel);
        ActionExecutedContext result = await next();

        if (result.Exception == null || result.ExceptionHandled)
        {
            unitOfWork.Commit();
        }
        else
        {
            unitOfWork.Rollback();
        }
    }
}