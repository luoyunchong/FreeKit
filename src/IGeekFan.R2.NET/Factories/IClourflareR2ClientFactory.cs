namespace IGeekFan.R2.NET.Factories;

public interface ICloudflareR2ClientFactory
{
    ICloudflareR2Client GetClient(string clientName, CancellationToken cancellationToken);
    bool TryRemoveClient(string clientName);
    ICloudflareR2Client RefreshClient(string clientName, CancellationToken cancellationToken);
}
