using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;

namespace IGeekFan.OpenIddict.FreeSql.Entities;

/// <summary>
/// OpenIddict 客户端应用实体
/// </summary>
[Index("idx_{tablename}_clientid", "ClientId", true)]
public class OpenIddictApplication
{
    [Column(IsPrimary = true, StringLength = 50)]
    public Guid Id { get; set; }

    [Column(StringLength = 50)]
    public string? ApplicationType { get; set; }

    [Column(StringLength = 100)]
    public string? ClientId { get; set; }

    [Column(StringLength = -2)]
    public string? ClientSecret { get; set; }

    [Column(StringLength = 50)]
    public string? ConsentType { get; set; }

    [Column(StringLength = 200)]
    public string? DisplayName { get; set; }

    [Column(StringLength = -2)]
    public string? DisplayNames { get; set; }

    [Column(StringLength = -2)]
    public string? Permissions { get; set; }

    [Column(StringLength = -2)]
    public string? PostLogoutRedirectUris { get; set; }

    [Column(StringLength = -2)]
    public string? Properties { get; set; }

    [Column(StringLength = -2)]
    public string? RedirectUris { get; set; }

    [Column(StringLength = -2)]
    public string? Requirements { get; set; }

    [Column(StringLength = 50)]
    public string? Type { get; set; }

    [Column(StringLength = 50)]
    public string? ConcurrencyToken { get; set; }
}
