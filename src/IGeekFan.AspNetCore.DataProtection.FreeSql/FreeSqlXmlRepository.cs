using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Xml.Linq;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql;

/// <summary>
/// An <see cref="IXmlRepository"/> backed by an FreeSql datastore.
/// </summary>
public class FreeSqlXmlRepository<TContext> : IXmlRepository
    where TContext : DbContext, IDataProtectionKeyContext
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of the <see cref="FreeSqlXmlRepository{TContext}"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public FreeSqlXmlRepository(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _logger = loggerFactory.CreateLogger<FreeSqlXmlRepository<TContext>>();
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public virtual IReadOnlyCollection<XElement> GetAllElements()
    {
        // forces complete enumeration
        return GetAllElementsCore().ToList().AsReadOnly();

        IEnumerable<XElement> GetAllElementsCore()
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TContext>();

                foreach (var key in context.DataProtectionKeys.Select.NoTracking().ToList())
                {
                    _logger.ReadingXmlFromKey(key.FriendlyName!, key.Xml);

                    if (!string.IsNullOrEmpty(key.Xml))
                    {
                        yield return XElement.Parse(key.Xml);
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    public void StoreElement(XElement element, string friendlyName)
    {
        using (var scope = _services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var newKey = new DataProtectionKey()
            {
                FriendlyName = friendlyName,
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };

            context.DataProtectionKeys.Add(newKey);
            _logger.LogSavingKeyToDbContext(friendlyName, typeof(TContext).Name);
            context.SaveChanges();
        }
    }
}