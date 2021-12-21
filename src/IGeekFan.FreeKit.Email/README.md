## IGeekFan.FreeKit


```csharp
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;

        public AccountService(
            IEmailSender emailSender,
            IOptions<MailKitOptions> options)
        {
            _emailSender = emailSender;
            _mailKitOptions = options.Value;
        }
        public async Task<string> SendComfirmEmail(RegisterDto registerDto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(registerDto.Nickname, registerDto.Email));
            message.Subject = $"VVLOG-请点击这里激活您的账号";

            string uuid = Guid.NewGuid().ToString();
            await RedisHelper.SetAsync("SendComfirmEmail-" + registerDto.Email, uuid, 30 * 60);

            message.Body = new TextPart("html")
            {
                Text = $@"{registerDto.Nickname},您好!</br>
感谢您在 vvlog  的注册，请点击这里激活您的账号：</br>
https:://www.xxx.com/accounts/confirm-email/{uuid}/
祝您使用愉快，使用过程中您有任何问题请及时联系我们。</br>"
            };

            await _emailSender.SendAsync(message);
            return "";
        }
```