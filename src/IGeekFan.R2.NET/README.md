# R2.NET

R2.NET is a .NET client for interacting with Cloudflare R2. This package provides a simple and efficient way to manage your Cloudflare R2 buckets and objects. It includes both direct client initiation and a factory pattern for generating and fetching clients.

- from [https://github.com/gageway1/R2.NET](https://github.com/gageway1/R2.NET)

## Installation

To install R2.NET, run the following command in your .NET project:

```bash
dotnet add package R2.NET
```

## Configuration
Add the following configuration to your appsettings.json:
```json
{
  "CloudflareR2": {
    "ApiBaseUri": "https://api.cloudflare.com/client/v4/accounts",
    "AccountId": "your-account-id",
    "ApiToken": "your-api-token"
  }
}
```

## Usage
### Example 1: Direct Client Initiation
This example demonstrates how to directly initiate the Cloudflare R2 client and use it to upload, retrieve, and delete blobs.

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

            // Upload Blob
            using var fileStream = File.OpenRead("path/to/your/file.zip");
            var blobUrl = await client.UploadBlobAsync("your-bucket-name", "your-blob-name.zip", fileStream, "application/zip");

            // Get Blob
            using var blobStream = await client.GetBlobAsync("your-bucket-name", "your-blob-name.zip");


            // Delete Blob
            await client.DeleteBlobAsync("your-bucket-name", "your-blob-name.zip");
        }
    }
}
```

## Example 2: Best Practice Using the Factory
This example demonstrates the best practice of using the factory pattern to generate and fetch clients.

### Step 1: Register the Services in DI
Add the following to your `Startup.cs` or `Program.cs`:
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

### Step 2: Use the factory in your service to create or fetch clients:
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

# Contributing
Contributions are welcome! Please feel free to submit a Pull Request on GitHub.

# License
This project is licensed under the MIT License. See the LICENSE file for details.