using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace IGeekFan.AspNetCore.DataProtection.FreeSql;

/// <summary>
/// Code first model used by <see cref="FreeSqlXmlRepository{TContext}"/>.
/// </summary>
public class DataProtectionKey
{
    /// <summary>
    /// The entity identifier of the <see cref="DataProtectionKey"/>.
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// The friendly name of the <see cref="DataProtectionKey"/>.
    /// </summary>
    [StringLength(500)]
    public string? FriendlyName { get; set; }

    /// <summary>
    /// The XML representation of the <see cref="DataProtectionKey"/>.
    /// </summary>
    [StringLength(-1)]
    public string? Xml { get; set; }
}
