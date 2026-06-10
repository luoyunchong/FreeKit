using FreeSql.DataAnnotations;

namespace IGeekFan.OpenIddict.FreeSql.Entities;

/// <summary>
/// OpenIddict 授权记录实体
/// </summary>
[Index("idx_{tablename}_subject", "Subject")]
[Index("idx_{tablename}_applicationid", "ApplicationId")]
public class OpenIddictAuthorization
{
    [Column(IsPrimary = true, StringLength = 50)]
    public Guid Id { get; set; }

    [Column(StringLength = 50)]
    public Guid? ApplicationId { get; set; }

    public DateTime? CreationDate { get; set; }

    [Column(StringLength = -2)]
    public string? Scopes { get; set; }

    [Column(StringLength = 50)]
    public string? Status { get; set; }

    [Column(StringLength = 50)]
    public string? Subject { get; set; }

    [Column(StringLength = 50)]
    public string? Type { get; set; }

    [Column(StringLength = -2)]
    public string? Properties { get; set; }

    [Column(StringLength = 50)]
    public string? ConcurrencyToken { get; set; }
}
