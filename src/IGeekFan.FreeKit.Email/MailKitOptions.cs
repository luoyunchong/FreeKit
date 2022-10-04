namespace IGeekFan.FreeKit.Email;

/// <summary>
///  邮件配置项
/// </summary>
public class MailKitOptions
{
    /// <summary>
    /// SMTP主机服务器地址
    /// SMTP Host Server address
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// SMTP主机服务器端口，默认值为25
    /// SMTP Host Server Port ,default is 25
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// 是否启用ssl，默认值为false
    /// enable ssl,default is false
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// 发送用户显示名称
    /// send user display name
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 发件人邮件
    /// send user name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 发件人授权密码
    /// send user password
    /// </summary>
    public string Password { get; set; }


    /// <summary>
    /// 要登录SMTP服务器的域名
    /// Domain name to login to SMTP server.
    /// </summary>
    public string Domain { get; set; }
}