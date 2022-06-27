using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql.Tests;

public class DataProtectionFreeSqlTests
{
    [Fact]
    public void CreateRepository_ThrowsIf_ContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FreeSqlXmlRepository<DataProtectionKeyContext>(null, null));
    }

    [Fact]
    public void StoreElement_PersistsData()
    {
        var element = XElement.Parse("<Element1/>");
        var friendlyName = "Element1";
        var key = new DataProtectionKey() { FriendlyName = friendlyName, Xml = element.ToString() };

        var services = GetServices(nameof(StoreElement_PersistsData));
        var service = new FreeSqlXmlRepository<DataProtectionKeyContext>(services, NullLoggerFactory.Instance);
        service.StoreElement(element, friendlyName);

        // Use a separate instance of the context to verify correct data was saved to database
        using (var context = services.CreateScope().ServiceProvider.GetRequiredService<DataProtectionKeyContext>())
        {
            Assert.Equal(1, context.DataProtectionKeys.Select.Count());
            Assert.Equal(key.FriendlyName, context.DataProtectionKeys.Select.First()?.FriendlyName);
            Assert.Equal(key.Xml, context.DataProtectionKeys.Select.First()?.Xml);
        }
    }

    [Fact]
    public void GetAllElements_ReturnsAllElements()
    {
        var element1 = XElement.Parse("<Element1/>");
        var element2 = XElement.Parse("<Element2/>");

        var services = GetServices(nameof(GetAllElements_ReturnsAllElements));
        var service1 = CreateRepo(services);
        service1.StoreElement(element1, "element1");
        service1.StoreElement(element2, "element2");

        // Use a separate instance of the context to verify correct data was saved to database
        var service2 = CreateRepo(services);
        var elements = service2.GetAllElements();
        Assert.Equal(2, elements.Count);
    }

    private FreeSqlXmlRepository<DataProtectionKeyContext> CreateRepo(IServiceProvider services)
        => new FreeSqlXmlRepository<DataProtectionKeyContext>(services, NullLoggerFactory.Instance);

    private IServiceProvider GetServices(string dbName)=> FreeUtil.GetFreeSqlServiceCollection<DataProtectionKeyContext>().BuildServiceProvider(validateScopes: true);
}