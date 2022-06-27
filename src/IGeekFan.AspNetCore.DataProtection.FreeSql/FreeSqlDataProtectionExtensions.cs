using FreeSql;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using IGeekFan.AspNetCore.DataProtection.FreeSql;

namespace Microsoft.AspNetCore.DataProtection;

/// <summary>
/// Extension method class for configuring instances of <see cref="FreeSqlXmlRepository{TContext}"/>
/// </summary>
public static class FreeSqlDataProtectionExtensions
{
    /// <summary>
    /// Configures the data protection system to persist keys to an FreeSql datastore
    /// </summary>
    /// <param name="builder">The <see cref="IDataProtectionBuilder"/> instance to modify.</param>
    /// <returns>The value <paramref name="builder"/>.</returns>
    public static IDataProtectionBuilder PersistKeysToDbContext<TContext>(this IDataProtectionBuilder builder)
        where TContext : DbContext, IDataProtectionKeyContext
    {
        builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
        {
            var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
            return new ConfigureOptions<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new FreeSqlXmlRepository<TContext>(services, loggerFactory);
            });
        });

        return builder;
    }


}
