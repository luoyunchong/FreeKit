using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Email;

/// <summary>
/// 邮件服务器提供器，决定发送邮件时使用哪些 SMTP 服务器
/// <para>默认实现：从 IOptions&lt;MailKitOptions&gt; 读取</para>
/// <para>可替换为：Redis、数据库、带限额轮询等自定义策略</para>
/// </summary>
public interface IEmailServerProvider
{
    /// <summary>
    /// 获取可用的 SMTP 服务器列表（按优先级排序）
    /// </summary>
    Task<List<MailKitOptions>> GetServersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 通知某服务器发送成功（可用于更新限额计数等）
    /// </summary>
    Task OnSendSuccessAsync(MailKitOptions server, CancellationToken cancellationToken = default);

    /// <summary>
    /// 通知某服务器发送失败（可用于熔断、降级等）
    /// </summary>
    Task OnSendFailureAsync(MailKitOptions server, CancellationToken cancellationToken = default);
}
