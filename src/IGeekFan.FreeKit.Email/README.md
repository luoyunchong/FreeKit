# Email 邮件

## IGeekFan.FreeKit.Email

该包是一个独立的开发包，内部由MailKit实现发送邮件。

1. 安装包

```bash
dotnet add package IGeekFan.FreeKit.Email
```

1).通过配置文件配置Email服务

- ConfigureServices方法

```csharp
    services.AddEmailSender(configuration);
```

- appsettings.json

```json
  "MailKitOptions":{
    "Host": "smtp.163.com",
    "Port": "25",
    "EnableSsl": true,
    "UserName": "igeekfan@163.com",
    "Password": "",
    "Domain": ""
  },
```

2).通过回调函数配置

```csharp
    services.AddEmailSender(r =>
    {
        r.Host = "smtp.163.com";
        r.Port = 25;
        r.EnableSsl = true;
        r.UserName = "igeekfan@163.com";
        r.Password = "";
        r.Domain = "";
    });
```

- 业务逻辑

```csharp
    public interface IAccountService
    {
        Task SendEmailCode(RegisterDto registerDto);
    }
    public class AccountService : IAccountService
    {
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;

        public AccountService(
            IEmailSender emailSender,
            IOptions<MailKitOptions> options)
        {
            _emailSender = emailSender;
            _mailKitOptions = options.Value;
        }
        public async Task SendEmailCode(RegisterDto registerDto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(registerDto.Nickname, registerDto.Email));
            message.Subject = $"VVLOG-你的验证码是";

            string uuid = Guid.NewGuid().ToString();
            await RedisHelper.SetAsync("SendEmailCode-" + registerDto.Email, uuid, 30 * 60);

            int rand6Value = new Random().Next(100000, 999999);

            message.Body = new TextPart("html")
            {
                Text = $@"{registerDto.Nickname},您好!</br>你此次验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{rand6Value}"
            };


            await _emailSender.SendAsync(message);
        }
    }
```

简单的实体，在发送之前应验证必填项、密码强度和邮件格式等

```csharp
    public class RegisterDto 
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; } 
   }
```
