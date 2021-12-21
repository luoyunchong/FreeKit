using IGeekFan.FreeKit.Extras.Dependency;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Email
{
    public interface IEmailSender : ITransientDependency
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="message">Mail to be sent</param>
        /// <param name="cancellationToken"></param>
        Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    }
}