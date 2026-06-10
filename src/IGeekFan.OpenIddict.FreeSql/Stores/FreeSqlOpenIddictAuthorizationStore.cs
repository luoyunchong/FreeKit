using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using FreeSql;
using IGeekFan.OpenIddict.FreeSql.Entities;
using OpenIddict.Abstractions;

namespace IGeekFan.OpenIddict.FreeSql.Stores;

/// <summary>
/// FreeSql 实现的 OpenIddict 授权记录存储
/// </summary>
public class FreeSqlOpenIddictAuthorizationStore : IOpenIddictAuthorizationStore<OpenIddictAuthorization>
{
    private readonly IBaseRepository<OpenIddictAuthorization, Guid> Repository;

    public FreeSqlOpenIddictAuthorizationStore(IBaseRepository<OpenIddictAuthorization, Guid> repository)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await Repository.Select.CountAsync(cancellationToken);
    }

    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<OpenIddictAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        var list = await Repository.Select.ToListAsync(cancellationToken);
        return query(list.AsQueryable()).LongCount();
    }

    public virtual async ValueTask CreateAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        await Repository.InsertAsync(authorization, cancellationToken);
    }

    public virtual async ValueTask DeleteAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        await Repository.DeleteAsync(authorization.Id, cancellationToken);
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorization> FindAsync(
        string? subject, string? client, string? status, string? type,
        ImmutableArray<string>? scopes, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Repository.Select
            .WhereIf(!string.IsNullOrEmpty(subject), x => x.Subject == subject)
            .WhereIf(!string.IsNullOrEmpty(client) && Guid.TryParse(client, out _), x => x.ApplicationId == Guid.Parse(client!))
            .WhereIf(!string.IsNullOrEmpty(status), x => x.Status == status)
            .WhereIf(!string.IsNullOrEmpty(type), x => x.Type == type);

        var items = await query.ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorization> FindByApplicationIdAsync(
        string identifier, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var appId)) yield break;
        var items = await Repository.Where(x => x.ApplicationId == appId).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async ValueTask<OpenIddictAuthorization?> FindByIdAsync(
        string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var id)) return null;
        return await Repository.Where(x => x.Id == id).FirstAsync(cancellationToken);
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorization> FindBySubjectAsync(
        string subject, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(subject);
        var items = await Repository.Where(x => x.Subject == subject).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictAuthorization>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        return query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).FirstOrDefault();
    }

    public virtual ValueTask<string?> GetApplicationIdAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.ApplicationId?.ToString());
    }

    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.CreationDate.HasValue ? new DateTimeOffset(authorization.CreationDate.Value) : null);
    }

    public virtual ValueTask<string?> GetIdAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.Id.ToString());
    }

    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
        OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeJsonProperties(authorization.Properties));
    }

    public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(
        OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeStringArray(authorization.Scopes));
    }

    public virtual ValueTask<string?> GetStatusAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.Status);
    }

    public virtual ValueTask<string?> GetSubjectAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.Subject);
    }

    public virtual ValueTask<string?> GetTypeAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.Type);
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorization> ListAsync(
        int? count, int? offset, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Repository.Select.OrderBy(x => x.Id);
        if (offset.HasValue) query = query.Skip(offset.Value);
        if (count.HasValue) query = query.Take(count.Value);
        var items = await query.ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictAuthorization>, TState, IQueryable<TResult>> query,
        TState state, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).ToList();
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual ValueTask SetApplicationIdAsync(OpenIddictAuthorization authorization, string? identifier, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.ApplicationId = string.IsNullOrEmpty(identifier) ? null : Guid.Parse(identifier);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetCreationDateAsync(OpenIddictAuthorization authorization, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.CreationDate = date?.UtcDateTime;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPropertiesAsync(OpenIddictAuthorization authorization,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.Properties = FreeSqlOpenIddictApplicationStore.SerializeJsonProperties(properties);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetScopesAsync(OpenIddictAuthorization authorization,
        ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.Scopes = FreeSqlOpenIddictApplicationStore.SerializeStringArray(scopes);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetStatusAsync(OpenIddictAuthorization authorization, string? status, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.Status = status;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetSubjectAsync(OpenIddictAuthorization authorization, string? subject, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.Subject = subject;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetTypeAsync(OpenIddictAuthorization authorization, string? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.Type = type;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<string?> GetConcurrencyTokenAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        return new(authorization.ConcurrencyToken);
    }

    public virtual ValueTask SetConcurrencyTokenAsync(OpenIddictAuthorization authorization, string? token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        authorization.ConcurrencyToken = token;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<OpenIddictAuthorization> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new(new OpenIddictAuthorization());
    }

    public virtual async ValueTask UpdateAsync(OpenIddictAuthorization authorization, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        await Repository.UpdateAsync(authorization, cancellationToken);
    }

    public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        long count = 0;
        var staleAuthorizations = await Repository.Where(
            x => x.CreationDate < threshold.UtcDateTime && x.Status != OpenIddictConstants.Statuses.Valid)
            .ToListAsync(cancellationToken);

        foreach (var authorization in staleAuthorizations)
        {
            await Repository.DeleteAsync(authorization.Id, cancellationToken);
            count++;
        }
        return count;
    }

    public virtual async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var appId)) return 0;
        var authorizations = await Repository.Where(x => x.ApplicationId == appId).ToListAsync(cancellationToken);
        foreach (var auth in authorizations)
        {
            auth.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(auth, cancellationToken);
        }
        return authorizations.Count;
    }

    public virtual async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(subject);
        var authorizations = await Repository.Where(x => x.Subject == subject).ToListAsync(cancellationToken);
        foreach (var auth in authorizations)
        {
            auth.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(auth, cancellationToken);
        }
        return authorizations.Count;
    }

    public virtual async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
    {
        var authorizations = await Repository.Select
            .WhereIf(!string.IsNullOrEmpty(subject), x => x.Subject == subject)
            .WhereIf(!string.IsNullOrEmpty(client) && Guid.TryParse(client, out _), x => x.ApplicationId == Guid.Parse(client!))
            .WhereIf(!string.IsNullOrEmpty(status), x => x.Status == status)
            .WhereIf(!string.IsNullOrEmpty(type), x => x.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var auth in authorizations)
        {
            auth.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(auth, cancellationToken);
        }
        return authorizations.Count;
    }
}
