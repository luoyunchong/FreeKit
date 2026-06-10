namespace IGeekFan.R2.NET.Configuration;

public class CloudflareR2Options
{
    public const string SettingsName = "CloudflareR2";
    public string Host { get; set; } = string.Empty;
    public string ApiBaseUri { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public double? PresignedUrlExpiryInMinutes { get; set; } = 15;
    public string Bucket { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
}
