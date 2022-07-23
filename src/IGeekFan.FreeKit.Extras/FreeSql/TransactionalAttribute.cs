// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 事务
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class TransactionalAttribute : Attribute
{
    /// <summary>
    /// 事务传播方式
    /// </summary>
    public Propagation Propagation { get; set; } = Propagation.Required;

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }
}
