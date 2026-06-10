using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.MultiTenancy;

public class HostTenantResolver : ITenantResolver
{
    private readonly string _tenantDomainSuffix;
    private readonly bool _useSubdomain;

    public HostTenantResolver(string? tenantDomainSuffix = null, bool useSubdomain = true)
    {
        _tenantDomainSuffix = tenantDomainSuffix ?? string.Empty;
        _useSubdomain = useSubdomain;
    }

    public int Priority => 40;

    public Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;

        if (string.IsNullOrEmpty(host))
        {
            return Task.FromResult<TenantInfo?>(null);
        }

        if (!string.IsNullOrEmpty(_tenantDomainSuffix) && host.EndsWith(_tenantDomainSuffix))
        {
            var subdomain = host[..^_tenantDomainSuffix.Length].TrimEnd('.');
            if (!string.IsNullOrEmpty(subdomain) && !subdomain.Contains('.'))
            {
                return Task.FromResult<TenantInfo?>(new TenantInfo { Code = subdomain });
            }
        }

        if (_useSubdomain)
        {
            var parts = host.Split('.');
            if (parts.Length >= 3)
            {
                return Task.FromResult<TenantInfo?>(new TenantInfo { Code = parts[0] });
            }
        }

        return Task.FromResult<TenantInfo?>(null);
    }
}
