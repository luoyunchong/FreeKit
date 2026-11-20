[English](README.md) | **简体中文**

# R2.NET

R2.NET 是用于访问 Cloudflare R2 的 .NET 客户端，提供管理 R2 存储桶和对象的方法。它支持直接初始化客户端，也支持使用工厂模式创建和获取客户端。

- 来自 [https://github.com/gageway1/R2.NET](https://github.com/gageway1/R2.NET)

## 安装

在 .NET 项目中运行以下命令安装 R2.NET：

```bash
dotnet add package R2.NET
```

## 配置
将以下配置添加到 appsettings.json：
```json
{
  "CloudflareR2": {
    "ApiBaseUri": "https://api.cloudflare.com/client/v4/accounts",
    "AccountId": "your-account-id",
    "ApiToken": "your-api-token"
  }
}
```

## 使用
### 示例一：直接初始化客户端
以下示例演示如何直接初始化 Cloudflare R2 客户端，并使用它上传、获取和删除对象。

```cs
using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace YourNamespace
{
    public class ClientUsage
    {
        public async Task RunAsync()
        {
            var options = Options.Create(new CloudflareR2Options
            {
                ApiBaseUri = "https://api.cloudflare.com/client/v4/accounts",
                AccountId = "your-account-id",
                ApiToken = "your-api-token"
            });

            var httpClient = new HttpClient();
            var client = new CloudflareR2Client(httpClient, options);

            // 上传对象
            using var fileStream = File.OpenRead("path/to/your/file.zip");
            var blobUrl = await client.UploadBlobAsync("your-bucket-name", "your-blob-name.zip", fileStream, "application/zip");

            // 获取对象
            using var blobStream = await client.GetBlobAsync("your-bucket-name", "your-blob-name.zip");


            // 删除对象
            await client.DeleteBlobAsync("your-bucket-name", "your-blob-name.zip");
        }
    }
}
```

## 示例二：使用工厂模式
以下示例演示如何使用工厂模式创建和获取客户端。

### 第一步：在依赖注入容器中注册服务
将以下内容添加到 `Startup.cs` 或 `Program.cs`：
```cs
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CloudflareR2Options>(Configuration.GetSection(CloudflareR2Options.SettingsName));
        services.AddSingleton<ICloudflareR2ClientFactory, CloudflareR2ClientFactory>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}
```

### 第二步：在服务中使用工厂创建或获取客户端
```cs
public class MyService
{
    private readonly ICloudflareR2ClientFactory _clientFactory;

    public MyService(ICloudflareR2ClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task UploadFileAsync(string clientName, string bucketName, string objectName, Stream fileStream)
    {
        var client = _clientFactory.GetClient(clientName);

        await client.UploadBlobAsync(bucketName, objectName, fileStream, "application/octet-stream");
    }
}
```

# 参与贡献
欢迎贡献！请在 GitHub 上提交 Pull Request。

# 许可证
本项目采用 MIT 许可证，详见 LICENSE 文件。