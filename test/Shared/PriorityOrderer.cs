namespace Microsoft.AspNetCore.Identity.Test;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

/// <summary>
/// Test priority
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="priority"></param>
    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    /// Priority
    /// </summary>
    public int Priority { get; private set; }
}