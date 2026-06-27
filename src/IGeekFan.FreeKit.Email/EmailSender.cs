using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IGeekFan.FreeKit.Email;

public class EmailSender : IEmailSender
{
    private readonly MailKitOptions _options;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptionsMonitor<MailKitOptions> options, ILogger<EmailSender> logger)
    {
        _options = options.CurrentValue;
        _logger = logger;
    }

    public void Send(MimeMessage message)
    {
        SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        EnsureFromAddress(message);

        var servers = _options.ToServers();
        if (servers.Count == 0)
            throw new InvalidOperationException("未配置任何邮件服务器，请设置 MailKitOptions 的 Host 或 Servers");

        Exception? lastException = null;
        foreach (var server in servers)
        {
            try
            {
                await SendWithServerAsync(message, server, cancellationToken);
                return;
            }
            catch (Exception ex) when (IsTransientError(ex))
            {
                lastException = ex;
                _logger.LogWarning(ex, "邮件服务器 [{Name}] ({Host}) 发送失败，尝试下一个", server.Name, server.Host);
            }
        }

        throw lastException ?? new InvalidOperationException("所有邮件服务器均不可用");
    }

    private void EnsureFromAddress(MimeMessage message)
    {
        if (message.From.Count > 0) return;

        var server = _options.ToServers().FirstOrDefault();
        var displayName = server?.DisplayName ?? _options.DisplayName;
        var userName = server?.UserName ?? _options.UserName;

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = userName;

        if (!string.IsNullOrWhiteSpace(userName))
            message.From.Add(new MailboxAddress(displayName, userName));
    }

    /// <summary>
    /// 使用指定服务器发送邮件
    /// </summary>
    public static async Task SendWithServerAsync(
        MimeMessage message, SmtpServerOption server, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        var secureOptions = server.EnableSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;

        await client.ConnectAsync(server.Host, server.Port, secureOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(server.UserName))
            await client.AuthenticateAsync(server.UserName, server.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private static bool IsTransientError(Exception ex) =>
        ex is SmtpCommandException or SmtpProtocolException
            or IOException or SocketException or TimeoutException;
}
