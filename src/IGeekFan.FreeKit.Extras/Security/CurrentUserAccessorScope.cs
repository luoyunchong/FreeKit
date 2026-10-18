namespace IGeekFan.FreeKit.Extras.Security;

/// <summary>
/// Preserves compatibility with custom accessors that implement only CurrentUser.
/// The built-in accessor supplies its own AsyncLocal-aware implementation.
/// </summary>
internal sealed class CurrentUserAccessorScope(ICurrentUserAccessor accessor, ICurrentUser? previous) : IDisposable
{
    private ICurrentUserAccessor? _accessor = accessor;

    public void Dispose()
    {
        var current = Interlocked.Exchange(ref _accessor, null);
        if (current != null) current.CurrentUser = previous;
    }
}
