// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Castle.DynamicProxy;
using FreeSql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Extras.FreeSql;

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
        var attribute = method.GetCustomAttributes(typeof(TransactionalAttribute), false).FirstOrDefault();
        if (attribute is TransactionalAttribute transaction)
        {
            if (transaction.IsolationLevel == null)
            {
                transaction.IsolationLevel = _unitOfWorkDefualtOptions.IsolationLevel;
            }
            if (transaction.Propagation == null)
            {
                transaction.Propagation = _unitOfWorkDefualtOptions.Propagation;
            }
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
            int? hashCode = _unitOfWork.GetHashCode();
            try
            {
                invocation.Proceed();
                _logger.LogInformation("----- 拦截同步执行的方法-事务 {HashCode} 提交前----- ", hashCode);
                _unitOfWork.Commit();
                _logger.LogInformation("----- 拦截同步执行的方法-事务 {HashCode} 提交成功----- ", hashCode);
            }
            catch
            {
                _logger.LogError("----- 拦截同步执行的方法-事务 {HashCode} 提交失败----- ", hashCode);
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
        string methodName = $"{invocation.MethodInvocationTarget.DeclaringType?.FullName}.{invocation.Method.Name}()";
        int? hashCode = _unitOfWork.GetHashCode();

        using (_logger.BeginScope("_unitOfWork:{hashCode}", hashCode))
        {
            _logger.LogInformation("----- async Task 开始事务{HashCode} {MethodName}----- ", hashCode, methodName);

            try
            {
                invocation.Proceed();
                //处理Task返回一个null值的情况会导致空指针
                if (invocation.ReturnValue != null)
                {
                    await (Task)invocation.ReturnValue;
                }
                _unitOfWork.Commit();
                _logger.LogInformation("----- async Task 事务 {HashCode} Commit----- ", hashCode);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                _logger.LogError("----- async Task 事务 {HashCode} Rollback----- ", hashCode);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
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
            string methodName = $"{invocation.MethodInvocationTarget.DeclaringType?.FullName}.{invocation.Method.Name}()";
            int hashCode = _unitOfWork.GetHashCode();
            _logger.LogInformation("----- async Task<TResult> 开始事务{HashCode} {MethodName}----- ", hashCode, methodName);
            try
            {
                invocation.Proceed();
                Task<TResult> task = (Task<TResult>)invocation.ReturnValue;
                result = await task;
                _unitOfWork.Commit();
                _logger.LogInformation("----- async Task<TResult> Commit事务{HashCode}----- ", hashCode);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                _logger.LogError("----- async Task<TResult> Rollback事务{HashCode}----- ", hashCode);
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