using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGeekFan.FreeKit.Email;

public static class EmailSenderDependencyInjection
{
    /// <summary>
    /// 配置Email服务
    /// </summary>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddEmailSender(this IServiceCollection services,IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        
        services.Configure<MailKitOptions>(configuration.GetSection("MailKitOptions"));
        services.TryAddTransient<IEmailSender, EmailSender>();
        return services;
    }  
    
    public static IServiceCollection AddEmailSender(this IServiceCollection services,Action<MailKitOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }
        
        services.Configure(configureOptions);
        services.TryAddTransient<IEmailSender, EmailSender>();
        return services;
    }
}