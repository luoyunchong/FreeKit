using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class QueryStringTenantResolver : ITenantResolver
{
    private readonly string _queryStringKey;

    public QueryStringTenantResolver(string queryStringKey = "tenantId")
    {
        _queryStringKey = queryStringKey;
    }

    public int Priority => 30;

    public Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var tenantIdValue = context.Request.Query[_queryStringKey].ToString();
        if (string.IsNullOrEmpty(tenantIdValue))
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
