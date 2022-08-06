﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public class UnitOfWorkDefualtOptions
{
    /// <summary>
    /// 事务传播方式
    /// </summary>
    public Propagation? Propagation { get; set; }

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }
}
