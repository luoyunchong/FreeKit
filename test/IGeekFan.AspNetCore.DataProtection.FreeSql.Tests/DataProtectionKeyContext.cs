using FreeSql;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql.Tests;

public class DataProtectionKeyContext : DbContext, IDataProtectionKeyContext
{
    public DataProtectionKeyContext(IFreeSql fsql, DbContextOptions options) : base(fsql, options) { }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}