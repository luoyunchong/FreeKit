using FreeSql.DataAnnotations;

namespace IGeekFan.OpenIddict.FreeSql.Entities;

/// <summary>
/// OpenIddict 令牌实体
/// </summary>
[Index("idx_{tablename}_referenceid", "ReferenceId", true)]
[Index("idx_{tablename}_subject", "Subject")]
[Index("idx_{tablename}_applicationid", "ApplicationId")]
[Index("idx_{tablename}_authorizationid", "AuthorizationId")]
public class OpenIddictToken
{
    [Column(IsPrimary = true, StringLength = 50)]
    public Guid Id { get; set; }

    [Column(StringLength = 50)]
    public Guid? ApplicationId { get; set; }

    [Column(StringLength = 50)]
    public Guid? AuthorizationId { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Column(StringLength = -2)]
    public string? Payload { get; set; }

    [Column(StringLength = -2)]
    public string? Properties { get; set; }

    public DateTime? RedemptionDate { get; set; }

    [Column(StringLength = 200)]
    public string? ReferenceId { get; set; }

    [Column(StringLength = 50)]
    public string? Status { get; set; }

    [Column(StringLength = 50)]
    public string? Subject { get; set; }

    [Column(StringLength = -2)]
    public string? Token { get; set; }

    [Column(StringLength = 200)]
    public string? Type { get; set; }

    [Column(StringLength = 50)]
    public string? ConcurrencyToken { get; set; }
}
