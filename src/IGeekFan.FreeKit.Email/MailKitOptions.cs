using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IGeekFan.FreeKit.Email;

/// <summary>
/// 邮件配置项（支持单服务器或多服务器模式）
/// <para>单服务器：直接设置 Host/Port/UserName/Password</para>
/// <para>多服务器：设置 Servers 列表，按顺序尝试直到发送成功</para>
/// </summary>
public class MailKitOptions
{
    /// <summary>
    /// SMTP主机服务器地址
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// SMTP主机服务器端口，默认值为25
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// 是否启用ssl，默认值为false
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// 发送用户显示名称
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 发件人邮件
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 发件人授权密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 要登录SMTP服务器的域名
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// 多服务器配置（优先使用，为空时回退到单服务器字段）
    /// </summary>
    public List<SmtpServerOption> Servers { get; set; } = new();

    /// <summary>
    /// 将当前配置转为 SmtpServerOption 列表（兼容单服务器模式）
    /// </summary>
    public List<SmtpServerOption> ToServers()
    {
        if (Servers is { Count: > 0 })
            return Servers;

        if (string.IsNullOrWhiteSpace(Host))
            return [];

        return [
            new SmtpServerOption
            {
                Name = "默认",
                Host = Host,
                Port = Port,
                EnableSsl = EnableSsl,
                DisplayName = DisplayName,
                UserName = UserName ?? string.Empty,
                Password = Password ?? string.Empty,
                Domain = Domain
            }
        ];
    }
}
