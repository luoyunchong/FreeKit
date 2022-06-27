using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql.Tests;
public class FreeSqlDataProtectionBuilderExtensionsTests
{

    [Fact]
    public void PersistKeysToFreeSql_UsesFreeSqlXmlRepository()
    {
        ServiceCollection? serviceCollection = FreeUtil.GetFreeSqlServiceCollection<DataProtectionKeyContext>();
        serviceCollection
            .AddDataProtection().PersistKeysToDbContext<DataProtectionKeyContext>();
        var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);
        var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();
        Assert.IsType<FreeSqlXmlRepository<DataProtectionKeyContext>>(keyManagementOptions.Value.XmlRepository);
    }
}
