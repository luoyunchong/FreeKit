// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 事务特性标签
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TransactionalAttribute : Attribute
{
    #region Constructor
    public TransactionalAttribute()
    {
    }

    public TransactionalAttribute(IsolationLevel? isolationLevel)
    {
        IsolationLevel = isolationLevel;
    }

    public TransactionalAttribute(bool isDisabled)
    {
        IsDisabled = isDisabled;
    }

    public TransactionalAttribute(Propagation propagation)
    {
        Propagation = propagation;
    }


    public TransactionalAttribute(Propagation propagation, IsolationLevel isolationLevel)
    {
        Propagation = propagation;
        IsolationLevel = isolationLevel;
    }

    public TransactionalAttribute(Propagation propagation, IsolationLevel isolationLevel, bool isDisabled)
    {
        Propagation = propagation;
        IsolationLevel = isolationLevel;
        IsDisabled = isDisabled;
    } 
    #endregion

    /// <summary>
    /// 事务传播方式
    /// </summary>
    public Propagation? Propagation { get; set; }

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// 是否禁用，默认为false
    /// </summary>
    public bool IsDisabled { get; set; }
}
