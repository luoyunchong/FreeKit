using IGeekFan.FreeKit.Email;
using MimeKit;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Email
{
    public class EmailSenderTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IEmailSender _emailSender;
        public EmailSenderTest(ITestOutputHelper testOut, IEmailSender emailSender)
        {
            _testOutputHelper = testOut;
            _emailSender = emailSender;
        }

        [Fact]
        public async Task SendAsyncTest()
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("Mr. luoyunchong", "luoyunchong@foxmail.com"));
            message.Subject = "How you doin' from SendAsyncTest?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"

            };
            await _emailSender.SendAsync(message);

            await Task.CompletedTask;
        }

        [Fact]
        public void SendTest()
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("Mr. luoyunchong", "luoyunchong@foxmail.com"));
            message.Subject = "How you doin' from SendTest?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"

            };
            _emailSender.Send(message);
        }
    }
}
