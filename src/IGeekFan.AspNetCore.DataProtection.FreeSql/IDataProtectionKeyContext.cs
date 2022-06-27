using FreeSql;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql;

/// <summary>
/// Interface used to store instances of <see cref="DataProtectionKey"/> in a <see cref="DbContext"/>
/// </summary>
public interface IDataProtectionKeyContext
{
    /// <summary>
    /// A collection of <see cref="DataProtectionKey"/>
    /// </summary>
    DbSet<DataProtectionKey> DataProtectionKeys { get; }
}
