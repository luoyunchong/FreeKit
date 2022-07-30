using IGeekFan.FreeKit.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Email
{
    public class EmailSenderTest 
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;
        public EmailSenderTest(ITestOutputHelper testOut, IEmailSender emailSender, IOptions<MailKitOptions> options)
        {
            testOutputHelper = testOut;
            _emailSender = emailSender;
            _mailKitOptions = options.Value;
        }

        [Fact]
        public async Task OutputTest()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            //message.From.Add(new MailboxAddress("igeekfan", "igeekfan@163.com"));
            message.To.Add(new MailboxAddress("Mrs. luoyunchong", "luoyunchong@foxmail.com"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"

            };
            await _emailSender.SendAsync(message);

            await Task.CompletedTask;
        }
    }
}
