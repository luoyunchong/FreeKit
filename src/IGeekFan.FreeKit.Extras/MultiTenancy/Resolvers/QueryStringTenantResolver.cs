using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class QueryStringTenantResolver : ITenantResolver
{
    private readonly string _queryStringKey;

    public QueryStringTenantResolver(string queryStringKey = "tenantId")
    {
        _queryStringKey = queryStringKey;
    }

    public QueryStringTenantResolver(IOptions<MultiTenancyOptions> options)
        : this(options?.Value.QueryStringKey ?? "tenantId")
    {
    }

    public int Priority => 30;

    public Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var tenantIdValue = context.Request.Query[_queryStringKey].ToString().Trim();
        if (string.IsNullOrWhiteSpace(tenantIdValue))
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        if (Guid.TryParse(tenantIdValue, out var tenantId))
        {
            return Task.FromResult<TenantInfo?>(new TenantInfo { Id = tenantId });
        }

        return Task.FromResult<TenantInfo?>(new TenantInfo { Code = tenantIdValue });
    }
}
