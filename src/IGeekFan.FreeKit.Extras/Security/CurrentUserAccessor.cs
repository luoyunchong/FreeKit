// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// Provides an implementation of <see cref="ICurrentUserAccessor" /> based on the current execution context.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private sealed class CurrentHolder
    {
        public ICurrentUser? Context;
    }
    private static readonly AsyncLocal<CurrentHolder?> _currentUser = new();

    /// <summary>
    /// 当前用户信息上下文
    /// </summary>
    /// <value></value>
    public ICurrentUser? CurrentUser
    {
        get => _currentUser.Value?.Context;
        set
        {
            var holder = _currentUser.Value;
            if (holder != null)
            {
                // Clear current ICurrentUser trapped in the AsyncLocals, as its done.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the ICurrentUser in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _currentUser.Value = new CurrentHolder { Context = value };
            }
        }
    }

    /// <inheritdoc />
    public IDisposable Change(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        var previous = _currentUser.Value;
        var current = new CurrentHolder { Context = currentUser };
        _currentUser.Value = current;
        return new UserScope(previous, current);
    }

    private sealed class UserScope(CurrentHolder? previous, CurrentHolder current) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            // 让继承此作用域的子任务也失去已结束请求的用户引用。
            if (_currentUser.Value is { } active) active.Context = null;
            current.Context = null;
            _currentUser.Value = previous;
        }
    }
}
