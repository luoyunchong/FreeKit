using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using FreeSql;
using IGeekFan.OpenIddict.FreeSql.Entities;
using OpenIddict.Abstractions;

namespace IGeekFan.OpenIddict.FreeSql.Stores;

/// <summary>
/// FreeSql 实现的 OpenIddict 资源/范围存储
/// </summary>
public class FreeSqlOpenIddictScopeStore : IOpenIddictScopeStore<OpenIddictScope>
{
    private readonly IBaseRepository<OpenIddictScope, Guid> Repository;

    public FreeSqlOpenIddictScopeStore(IBaseRepository<OpenIddictScope, Guid> repository)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await Repository.Select.CountAsync(cancellationToken);
    }

    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<OpenIddictScope>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        var list = await Repository.Select.ToListAsync(cancellationToken);
        return query(list.AsQueryable()).LongCount();
    }

    public virtual async ValueTask CreateAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        await Repository.InsertAsync(scope, cancellationToken);
    }

    public virtual async ValueTask DeleteAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        await Repository.DeleteAsync(scope.Id, cancellationToken);
    }

    public virtual async ValueTask<OpenIddictScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var id)) return null;
        return await Repository.Where(x => x.Id == id).FirstAsync(cancellationToken);
    }

    public virtual async ValueTask<OpenIddictScope?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        return await Repository.Where(x => x.Name == name).FirstAsync(cancellationToken);
    }

    public virtual IAsyncEnumerable<OpenIddictScope> FindByResourceAsync(
        string resource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(resource);
        // 先按模糊匹配过滤，再精确匹配
        return FindByJsonArrayFieldAsync(nameof(OpenIddictScope.Resources), resource, cancellationToken);
    }

    private async IAsyncEnumerable<OpenIddictScope> FindByJsonArrayFieldAsync(
        string fieldName, string value, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var scopes = await Repository.Select.ToListAsync(cancellationToken);
        foreach (var scope in scopes)
        {
            var fieldValue = fieldName switch
            {
                nameof(OpenIddictScope.Resources) => scope.Resources,
                _ => null
            };

            if (fieldValue is not null)
            {
                var items = FreeSqlOpenIddictApplicationStore.DeserializeStringArray(fieldValue);
                if (items.Contains(value, StringComparer.Ordinal))
                {
                    yield return scope;
                }
            }
        }
    }

    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictScope>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        return query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).FirstOrDefault();
    }

    public virtual ValueTask<string?> GetDescriptionAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(scope.Description);
    }

    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(
        OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeCulturedStrings(scope.Descriptions));
    }

    public virtual ValueTask<string?> GetDisplayNameAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(scope.DisplayName);
    }

    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(
        OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeCulturedStrings(scope.DisplayNames));
    }

    public virtual ValueTask<string?> GetIdAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(scope.Id.ToString());
    }

    public virtual ValueTask<string?> GetNameAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(scope.Name);
    }

    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
        OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeJsonProperties(scope.Properties));
    }

    public virtual ValueTask<ImmutableArray<string>> GetResourcesAsync(
        OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(FreeSqlOpenIddictApplicationStore.DeserializeStringArray(scope.Resources));
    }

    public virtual async IAsyncEnumerable<OpenIddictScope> ListAsync(
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

    public virtual async IAsyncEnumerable<OpenIddictScope> FindByNamesAsync(
        ImmutableArray<string> names, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (names.IsDefaultOrEmpty) yield break;
        var nameList = names.ToList();
        var items = await Repository.Where(x => nameList.Contains(x.Name!)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictScope>, TState, IQueryable<TResult>> query,
        TState state, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).ToList();
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual ValueTask SetDescriptionAsync(OpenIddictScope scope, string? description, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.Description = description;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetDescriptionsAsync(OpenIddictScope scope,
        ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.Descriptions = FreeSqlOpenIddictApplicationStore.SerializeCulturedStrings(descriptions);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetDisplayNameAsync(OpenIddictScope scope, string? name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.DisplayName = name;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetDisplayNamesAsync(OpenIddictScope scope,
        ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.DisplayNames = FreeSqlOpenIddictApplicationStore.SerializeCulturedStrings(names);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetNameAsync(OpenIddictScope scope, string? name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.Name = name;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPropertiesAsync(OpenIddictScope scope,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.Properties = FreeSqlOpenIddictApplicationStore.SerializeJsonProperties(properties);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetResourcesAsync(OpenIddictScope scope,
        ImmutableArray<string> resources, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.Resources = FreeSqlOpenIddictApplicationStore.SerializeStringArray(resources);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<string?> GetConcurrencyTokenAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        return new(scope.ConcurrencyToken);
    }

    public virtual ValueTask SetConcurrencyTokenAsync(OpenIddictScope scope, string? token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.ConcurrencyToken = token;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<OpenIddictScope> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new(new OpenIddictScope());
    }

    public virtual async ValueTask UpdateAsync(OpenIddictScope scope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scope);
        await Repository.UpdateAsync(scope, cancellationToken);
    }
}
