using FreeSql;
using IGeekFan.AspNetCore.DataProtection.FreeSql;

namespace IGeekFan.FreeKit.Sample
{
    public class DataProtectionKeyContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtectionKeyContext(IFreeSql fsql, DbContextOptions options) : base(fsql, options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
