using System.ComponentModel.DataAnnotations;

namespace IGeekFan.FreeKit.Email;

/// <summary>
/// 单个 SMTP 服务器配置
/// </summary>
public class SmtpServerOption
{
    /// <summary>
    /// 服务器名称（仅用于日志标识，如 "163"、"QQ"）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SMTP 主机地址
    /// </summary>
    [Required]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP 端口，默认 465
    /// </summary>
    public int Port { get; set; } = 465;

    /// <summary>
    /// 是否启用 SSL
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// 发件人显示名称
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// SMTP 用户名（通常为邮箱地址）
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP 密码或授权码
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 登录 SMTP 服务器的域名（可选）
    /// </summary>
    public string? Domain { get; set; }
}
