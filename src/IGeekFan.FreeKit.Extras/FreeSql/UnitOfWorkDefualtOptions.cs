﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 默认事务配置
/// </summary>
public class UnitOfWorkDefaultOptions
{
    /// <summary>
    /// 事务传播方式
    /// </summary>
    public Propagation? Propagation { get; set; }

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// 是否发布领域事件
    /// </summary>
    public bool PublishDomainEvent { get; set; } = true;
}
