using IGeekFan.FreeKit.Extras.Dependency;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Email
{
    public interface IEmailSender : ITransientDependency
    {
        /// <summary>
        /// Send an Email.
        /// </summary>
        /// <param name="message"></param>
        void Send(MimeMessage message);

        /// <summary>
        /// Sends an Email.
        /// </summary>
        /// <param name="message">Mail to be sent</param>
        /// <param name="cancellationToken"></param>
        Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    }
}