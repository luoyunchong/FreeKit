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
    public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services.Configure<MailKitOptions>(configuration.GetSection("MailKitOptions"));
        services.TryAddTransient<IEmailServerProvider, EmailServerProvider>();
        services.TryAddTransient<IEmailSender, EmailSender>();
        return services;
    }

    /// <summary>
    /// 配置Email服务（回调方式）
    /// </summary>
    public static IServiceCollection AddEmailSender(this IServiceCollection services, Action<MailKitOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        services.TryAddTransient<IEmailServerProvider, EmailServerProvider>();
        services.TryAddTransient<IEmailSender, EmailSender>();
        return services;
    }

    /// <summary>
    /// 配置Email服务，使用自定义服务器提供器
    /// </summary>
    public static IServiceCollection AddEmailSender<TProvider>(this IServiceCollection services, IConfiguration configuration)
        where TProvider : class, IEmailServerProvider
    {
        services.Configure<MailKitOptions>(configuration.GetSection("MailKitOptions"));
        services.TryAddTransient<IEmailServerProvider, TProvider>();
        services.TryAddTransient<IEmailSender, EmailSender>();
        return services;
    }
}