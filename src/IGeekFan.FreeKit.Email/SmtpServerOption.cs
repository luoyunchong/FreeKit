namespace IGeekFan.FreeKit.Email;

/// <summary>
/// 单个 SMTP 服务器配置，继承 MailKitOptions 基础字段，仅增加 Name 标识
/// </summary>
public class SmtpServerOption : MailKitOptions
{
    /// <summary>
    /// 服务器名称（仅用于日志标识，如 "163"、"QQ"）
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
