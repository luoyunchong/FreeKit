using FreeSql.DataAnnotations;

namespace IGeekFan.OpenIddict.FreeSql.Entities;

/// <summary>
/// OpenIddict 资源/范围实体
/// </summary>
[Index("idx_{tablename}_name", "Name", true)]
public class OpenIddictScope
{
    [Column(IsPrimary = true, StringLength = 50)]
    public Guid Id { get; set; }

    [Column(StringLength = -2)]
    public string? Description { get; set; }

    [Column(StringLength = -2)]
    public string? Descriptions { get; set; }

    [Column(StringLength = 200)]
    public string? DisplayName { get; set; }

    [Column(StringLength = -2)]
    public string? DisplayNames { get; set; }

    [Column(StringLength = 200)]
    public string? Name { get; set; }

    [Column(StringLength = -2)]
    public string? Properties { get; set; }

    [Column(StringLength = -2)]
    public string? Resources { get; set; }

    [Column(StringLength = 50)]
    public string? ConcurrencyToken { get; set; }
}
