using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IGeekFan.FreeKit.Email;

public class EmailSender : IEmailSender
{
    private readonly MailKitOptions _options;

    public EmailSender(IOptionsMonitor<MailKitOptions> options)
    {
        _options = options.CurrentValue;
    }

    public void Send(MimeMessage message)
    {
        using SmtpClient client = new SmtpClient();

        client.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls;//| SslProtocols.Ssl2 | SslProtocols.Ssl3
        client.Connect(_options.Host, _options.Port, _options.EnableSsl == true ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

        NetworkCredential credential = !string.IsNullOrEmpty(_options.Domain)
            ? new NetworkCredential(_options.UserName, _options.Password, _options.Domain)
            : new NetworkCredential(_options.UserName, _options.Password);

        client.Authenticate(credential);
        client.Send(message);
        client.Disconnect(true);
    }

    public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        using SmtpClient client = new SmtpClient();

        client.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls;//| SslProtocols.Ssl2 | SslProtocols.Ssl3
        await client.ConnectAsync(_options.Host, _options.Port, _options.EnableSsl == true ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

        NetworkCredential credential = !string.IsNullOrEmpty(_options.Domain)
            ? new NetworkCredential(_options.UserName, _options.Password, _options.Domain)
            : new NetworkCredential(_options.UserName, _options.Password);

        await client.AuthenticateAsync(credential, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}