using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using FreeSql;
using IGeekFan.OpenIddict.FreeSql.Entities;
using OpenIddict.Abstractions;

namespace IGeekFan.OpenIddict.FreeSql.Stores;

/// <summary>
/// FreeSql 实现的 OpenIddict 客户端应用存储
/// </summary>
public class FreeSqlOpenIddictApplicationStore : IOpenIddictApplicationStore<OpenIddictApplication>
{
    private readonly IBaseRepository<OpenIddictApplication, Guid> Repository;

    public FreeSqlOpenIddictApplicationStore(IBaseRepository<OpenIddictApplication, Guid> repository)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await Repository.Select.CountAsync(cancellationToken);
    }

    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<OpenIddictApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        var list = await Repository.Select.ToListAsync(cancellationToken);
        return query(list.AsQueryable()).LongCount();
    }

    public virtual async ValueTask CreateAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        await Repository.InsertAsync(application, cancellationToken);
    }

    public virtual async ValueTask DeleteAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        await Repository.DeleteAsync(application.Id, cancellationToken);
    }

    public virtual async ValueTask<OpenIddictApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        if (!Guid.TryParse(identifier, out var id)) return null;
        return await Repository.Where(x => x.Id == id).FirstAsync(cancellationToken);
    }

    public virtual async ValueTask<OpenIddictApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);
        return await Repository.Where(x => x.ClientId == identifier).FirstAsync(cancellationToken);
    }

    public virtual IAsyncEnumerable<OpenIddictApplication> FindByPostLogoutRedirectUriAsync(
        string uri, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        // 在内存中过滤 JSON 字段
        return FindByJsonArrayFieldAsync(nameof(OpenIddictApplication.PostLogoutRedirectUris), uri, cancellationToken);
    }

    public virtual IAsyncEnumerable<OpenIddictApplication> FindByRedirectUriAsync(
        string uri, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        return FindByJsonArrayFieldAsync(nameof(OpenIddictApplication.RedirectUris), uri, cancellationToken);
    }

    private async IAsyncEnumerable<OpenIddictApplication> FindByJsonArrayFieldAsync(
        string fieldName, string value, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // 先按模糊匹配过滤，再精确匹配
        var applications = await Repository.Select.ToListAsync(cancellationToken);
        foreach (var app in applications)
        {
            var fieldValue = fieldName switch
            {
                nameof(OpenIddictApplication.RedirectUris) => app.RedirectUris,
                nameof(OpenIddictApplication.PostLogoutRedirectUris) => app.PostLogoutRedirectUris,
                _ => null
            };

            if (fieldValue is not null)
            {
                var items = DeserializeStringArray(fieldValue);
                if (items.Contains(value, StringComparer.Ordinal))
                {
                    yield return app;
                }
            }
        }
    }

    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictApplication>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        return query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).FirstOrDefault();
    }

    public virtual ValueTask<string?> GetApplicationTypeAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.ApplicationType);
    }

    public virtual ValueTask<string?> GetClientIdAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.ClientId);
    }

    public virtual ValueTask<string?> GetClientSecretAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.ClientSecret);
    }

    public virtual ValueTask<string?> GetClientTypeAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.Type);
    }

    public virtual ValueTask<string?> GetConsentTypeAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.ConsentType);
    }

    public virtual ValueTask<string?> GetDisplayNameAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.DisplayName);
    }

    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeCulturedStrings(application.DisplayNames));
    }

    public virtual ValueTask<string?> GetIdAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.Id.ToString());
    }

    public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeStringArray(application.Permissions));
    }

    public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeStringArray(application.PostLogoutRedirectUris));
    }

    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeJsonProperties(application.Properties));
    }

    public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeStringArray(application.RedirectUris));
    }

    public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(DeserializeStringArray(application.Requirements));
    }

    public virtual async IAsyncEnumerable<OpenIddictApplication> ListAsync(
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
        Func<IQueryable<OpenIddictApplication>, TState, IQueryable<TResult>> query,
        TState state, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = query((await Repository.Select.ToListAsync(cancellationToken)).AsQueryable(), state).ToList();
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public virtual ValueTask SetApplicationTypeAsync(OpenIddictApplication application, string? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.ApplicationType = type;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetClientIdAsync(OpenIddictApplication application, string? identifier, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.ClientId = identifier;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetClientSecretAsync(OpenIddictApplication application, string? secret, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.ClientSecret = secret;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetClientTypeAsync(OpenIddictApplication application, string? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.Type = type;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetConsentTypeAsync(OpenIddictApplication application, string? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.ConsentType = type;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetDisplayNameAsync(OpenIddictApplication application, string? name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.DisplayName = name;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetDisplayNamesAsync(OpenIddictApplication application,
        ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.DisplayNames = SerializeCulturedStrings(names);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPermissionsAsync(OpenIddictApplication application,
        ImmutableArray<string> permissions, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.Permissions = SerializeStringArray(permissions);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPostLogoutRedirectUrisAsync(OpenIddictApplication application,
        ImmutableArray<string> uris, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.PostLogoutRedirectUris = SerializeStringArray(uris);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetPropertiesAsync(OpenIddictApplication application,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.Properties = SerializeJsonProperties(properties);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetRedirectUrisAsync(OpenIddictApplication application,
        ImmutableArray<string> uris, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.RedirectUris = SerializeStringArray(uris);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetRequirementsAsync(OpenIddictApplication application,
        ImmutableArray<string> requirements, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.Requirements = SerializeStringArray(requirements);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<string?> GetConcurrencyTokenAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        return new(application.ConcurrencyToken);
    }

    public virtual ValueTask SetConcurrencyTokenAsync(OpenIddictApplication application, string? token, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        application.ConcurrencyToken = token;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<OpenIddictApplication> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new(new OpenIddictApplication());
    }

    public virtual async ValueTask UpdateAsync(OpenIddictApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        await Repository.UpdateAsync(application, cancellationToken);
    }

    public virtual ValueTask<Microsoft.IdentityModel.Tokens.JsonWebKeySet?> GetJsonWebKeySetAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        return new((Microsoft.IdentityModel.Tokens.JsonWebKeySet?)null);
    }

    public virtual ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(
        OpenIddictApplication application, CancellationToken cancellationToken)
    {
        return new(ImmutableDictionary<string, string>.Empty);
    }

    public virtual ValueTask SetJsonWebKeySetAsync(OpenIddictApplication application,
        Microsoft.IdentityModel.Tokens.JsonWebKeySet? jwks, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask SetSettingsAsync(OpenIddictApplication application,
        ImmutableDictionary<string, string> settings, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    #region JSON 序列化辅助

    internal static ImmutableArray<string> DeserializeStringArray(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return ImmutableArray<string>.Empty;
        return JsonSerializer.Deserialize<ImmutableArray<string>>(json);
    }

    internal static string? SerializeStringArray(ImmutableArray<string> values)
    {
        if (values.IsDefaultOrEmpty) return null;
        return JsonSerializer.Serialize(values);
    }

    internal static ImmutableDictionary<string, JsonElement> DeserializeJsonProperties(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return ImmutableDictionary<string, JsonElement>.Empty;
        return JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(json)
               ?? ImmutableDictionary<string, JsonElement>.Empty;
    }

    internal static string? SerializeJsonProperties(ImmutableDictionary<string, JsonElement> properties)
    {
        if (properties is null || properties.IsEmpty) return null;
        return JsonSerializer.Serialize(properties);
    }

    internal static ImmutableDictionary<CultureInfo, string> DeserializeCulturedStrings(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return ImmutableDictionary<CultureInfo, string>.Empty;
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (dict is null) return ImmutableDictionary<CultureInfo, string>.Empty;
        return dict.ToImmutableDictionary(
            kvp => CultureInfo.GetCultureInfo(kvp.Key),
            kvp => kvp.Value);
    }

    internal static string? SerializeCulturedStrings(ImmutableDictionary<CultureInfo, string> values)
    {
        if (values is null || values.IsEmpty) return null;
        var dict = values.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
        return JsonSerializer.Serialize(dict);
    }

    #endregion
}
