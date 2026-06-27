using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace IGeekFan.FreeKit.Email;

/// <summary>
/// 默认邮件服务器提供器，从 IOptions&lt;MailKitOptions&gt; 读取
/// </summary>
public class EmailServerProvider : IEmailServerProvider
{
    private readonly MailKitOptions _options;

    public EmailServerProvider(IOptionsMonitor<MailKitOptions> options)
    {
        _options = options.CurrentValue;
    }

    public Task<List<MailKitOptions>> GetServersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_options.ToServers());
    }

    public Task OnSendSuccessAsync(MailKitOptions server, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task OnSendFailureAsync(MailKitOptions server, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
