using System.Collections.Immutable;
using System.Text.Json;
using FreeSql;
using IGeekFan.OpenIddict.FreeSql.Entities;
using OpenIddict.Abstractions;

namespace IGeekFan.OpenIddict.FreeSql.Stores;

/// <summary>
/// FreeSql 实现的 OpenIddict 令牌存储
/// </summary>
public class FreeSqlOpenIddictTokenStore : IOpenIddictTokenStore<OpenIddictToken>
{
    private readonly IBaseRepository<OpenIddictToken, Guid> Repository;

    public FreeSqlOpenIddictTokenStore(IBaseRepository<OpenIddictToken, Guid> repository)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await Repository.Select.CountAsync(cancellationToken);
    }

    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<OpenIddictToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        var list = await Repository.Select.ToListAsync(cancellationToken);
        return query(list.AsQueryable()).LongCount();
    }

    public virtual async ValueTask CreateAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        await Repository.InsertAsync(token, cancellationToken);
    }

    public virtual async ValueTask DeleteAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        await Repository.DeleteAsync(token.Id, cancellationToken);
    }

    public virtual async IAsyncEnumerable<OpenIddictToken> FindByApplicationIdAsync(
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

    public virtual async IAsyncEnumerable<OpenIddictToken> FindByAuthorizationIdAsync(
        string identifier, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var authId)) yield break;
        var items = await Repository.Where(x => x.AuthorizationId == authId).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async ValueTask<OpenIddictToken?> FindByIdAsync(
        string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var id)) return null;
        return await Repository.Where(x => x.Id == id).FirstAsync(cancellationToken);
    }

    public virtual async ValueTask<OpenIddictToken?> FindByReferenceIdAsync(
        string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);

        string normalizedIdentifier = NormalizeReferenceId(identifier)!;
        string legacyIdentifier = ToLegacyReferenceId(normalizedIdentifier)!;

        return await Repository.Where(x => x.ReferenceId == identifier
                                           || x.ReferenceId == normalizedIdentifier
                                           || x.ReferenceId == legacyIdentifier)
            .FirstAsync(cancellationToken);
    }

    public virtual async IAsyncEnumerable<OpenIddictToken> FindBySubjectAsync(
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
        Func<IQueryable<OpenIddictToken>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        return query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).FirstOrDefault();
    }

    public virtual ValueTask<string?> GetApplicationIdAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.ApplicationId?.ToString());
    }

    public virtual ValueTask<string?> GetAuthorizationIdAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.AuthorizationId?.ToString());
    }

    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.CreationDate.HasValue ? DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc) : null);
    }

    public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.ExpirationDate.HasValue ? DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc) : null);
    }

    public virtual ValueTask<string?> GetIdAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Id.ToString());
    }

    public virtual ValueTask<string?> GetPayloadAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Payload);
    }

    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
        OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeJsonProperties(token.Properties));
    }

    public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.RedemptionDate.HasValue ? DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc) : null);
    }

    public virtual ValueTask<string?> GetReferenceIdAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.ReferenceId);
    }

    public virtual ValueTask<string?> GetStatusAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Status);
    }

    public virtual ValueTask<string?> GetSubjectAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Subject);
    }

    public virtual ValueTask<string?> GetTokenAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Token);
    }

    public virtual ValueTask<string?> GetTypeAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.Type);
    }

    public virtual async IAsyncEnumerable<OpenIddictToken> ListAsync(
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
        Func<IQueryable<OpenIddictToken>, TState, IQueryable<TResult>> query,
        TState state, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).ToList();
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual ValueTask SetApplicationIdAsync(OpenIddictToken token, string? identifier, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.ApplicationId = string.IsNullOrEmpty(identifier) ? null : Guid.Parse(identifier);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetAuthorizationIdAsync(OpenIddictToken token, string? identifier, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.AuthorizationId = string.IsNullOrEmpty(identifier) ? null : Guid.Parse(identifier);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetCreationDateAsync(OpenIddictToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.CreationDate = date?.UtcDateTime;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetExpirationDateAsync(OpenIddictToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.ExpirationDate = date?.UtcDateTime;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPayloadAsync(OpenIddictToken token, string? payload, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Payload = payload;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPropertiesAsync(OpenIddictToken token,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Properties = FreeSqlOpenIddictApplicationStore.SerializeJsonProperties(properties);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetRedemptionDateAsync(OpenIddictToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.RedemptionDate = date?.UtcDateTime;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetReferenceIdAsync(OpenIddictToken token, string? identifier, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.ReferenceId = identifier;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetStatusAsync(OpenIddictToken token, string? status, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Status = status;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetSubjectAsync(OpenIddictToken token, string? subject, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Subject = subject;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetTokenAsync(OpenIddictToken token, string? value, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Token = value;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetTypeAsync(OpenIddictToken token, string? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Type = type;
        return ValueTask.CompletedTask;
    }

    public virtual async IAsyncEnumerable<OpenIddictToken> FindAsync(
        string? subject, string? client, string? status, string? type, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
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

    public virtual ValueTask<string?> GetConcurrencyTokenAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        return new(token.ConcurrencyToken);
    }

    public virtual ValueTask SetConcurrencyTokenAsync(OpenIddictToken token, string? value, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.ConcurrencyToken = value;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<OpenIddictToken> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new(new OpenIddictToken());
    }

    public virtual async ValueTask UpdateAsync(OpenIddictToken token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(token);
        await Repository.UpdateAsync(token, cancellationToken);
    }

    public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        long count = 0;
        var staleTokens = await Repository.Where(
            x => x.CreationDate < threshold.UtcDateTime
            && (x.Status == OpenIddictConstants.Statuses.Redeemed
                || x.Status == OpenIddictConstants.Statuses.Revoked))
            .ToListAsync(cancellationToken);

        foreach (var token in staleTokens)
        {
            await Repository.DeleteAsync(token.Id, cancellationToken);
            count++;
        }
        return count;
    }

    public virtual async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
    {
        var tokens = await Repository.Select
            .WhereIf(!string.IsNullOrEmpty(subject), x => x.Subject == subject)
            .WhereIf(!string.IsNullOrEmpty(client) && Guid.TryParse(client, out _), x => x.ApplicationId == Guid.Parse(client!))
            .WhereIf(!string.IsNullOrEmpty(status), x => x.Status == status)
            .WhereIf(!string.IsNullOrEmpty(type), x => x.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(token, cancellationToken);
        }
        return tokens.Count;
    }

    public virtual async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var appId)) return 0;
        var tokens = await Repository.Where(x => x.ApplicationId == appId).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(token, cancellationToken);
        }
        return tokens.Count;
    }

    public virtual async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var authId)) return 0;
        var tokens = await Repository.Where(x => x.AuthorizationId == authId).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(token, cancellationToken);
        }
        return tokens.Count;
    }

    public virtual async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(subject);
        var tokens = await Repository.Where(x => x.Subject == subject).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Status = OpenIddictConstants.Statuses.Revoked;
            await Repository.UpdateAsync(token, cancellationToken);
        }
        return tokens.Count;
    }

    private static string? NormalizeReferenceId(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return identifier;
        }

        return identifier.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    private static string? ToLegacyReferenceId(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return identifier;
        }

        string value = identifier.Replace('-', '+').Replace('_', '/');
        int remainder = value.Length % 4;
        if (remainder != 0)
        {
            value = value.PadRight(value.Length + (4 - remainder), '=');
        }

        return value;
    }

}
