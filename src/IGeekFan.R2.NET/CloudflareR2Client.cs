using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using IGeekFan.R2.NET.Configuration;
using IGeekFan.R2.NET.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IGeekFan.R2.NET;

public class CloudflareR2Client : ICloudflareR2Client
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<CloudflareR2Options> _options;
    private readonly string _bucketName;
    private readonly TimeSpan _presignedUrlExpiry;
    private readonly ILogger<CloudflareR2Client> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public CloudflareR2Client(string bucketName, IOptions<CloudflareR2Options> options, ILogger<CloudflareR2Client> logger, IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _bucketName = bucketName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _presignedUrlExpiry = TimeSpan.FromMinutes(_options.Value.PresignedUrlExpiryInMinutes ?? 15);
        var s3ClientFactory = new AmazonS3ClientFactory(_options);
        _s3Client = s3ClientFactory.GetClient(_bucketName, CancellationToken.None);
    }

    public async Task<string> UploadBlobAsync(Stream blob, string blobName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting upload for {BlobName}", blobName);
            string presignedUrl = await GeneratePresignedUrl(_bucketName, blobName, HttpVerb.PUT, _presignedUrlExpiry);
            string uploadUrl = presignedUrl.Split('?')[0];
            await UploadFileUsingPresignedUrl(presignedUrl, blob);
            _logger.LogInformation("Upload successful for {BlobName}", blobName);
            return uploadUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blob {BlobName}", blobName);
            throw;
        }
    }
    private static async Task UploadFileUsingPresignedUrl(string presignedUrl, Stream stream)
    {

        var handler = new HttpClientHandler();
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            return true;
        };

        using var httpClient = new HttpClient(handler);
        using var content = new StreamContent(stream);
        //content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        var response = await httpClient.PutAsync(presignedUrl, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> GetBlobAsync(string blobName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting blob {BlobName}", blobName);
            string presignedUrl = await GeneratePresignedUrl(_bucketName, blobName, HttpVerb.GET, _presignedUrlExpiry);
            var stream = await GetFileUsingPresignedUrl(presignedUrl, _httpClientFactory);
            _logger.LogInformation("Got blob {BlobName}", blobName);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blob {BlobName}", blobName);
            throw;
        }
    }
    private static async Task<Stream> GetFileUsingPresignedUrl(string presignedUrl, IHttpClientFactory httpClientFactory)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(presignedUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting blob {BlobName}", blobName);
            string presignedUrl = await GeneratePresignedUrl(_bucketName, blobName, HttpVerb.DELETE, _presignedUrlExpiry);
            await DeleteFileUsingPresignedUrl(presignedUrl, _httpClientFactory);
            _logger.LogInformation("Deleted blob {BlobName}", blobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob {BlobName}", blobName);
            throw;
        }
    }
    private static async Task DeleteFileUsingPresignedUrl(string presignedUrl, IHttpClientFactory httpClientFactory)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, presignedUrl);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<string> GeneratePresignedUrl(string bucketName, string objectName, HttpVerb verb, TimeSpan expiry)
    {
        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;

            _logger.LogInformation("Generating presigned URL for {ObjectName} with verb {Verb}", objectName, verb);
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectName,
                Verb = verb,
                Expires = DateTime.UtcNow.Add(expiry)
            };
            return await _s3Client.GetPreSignedURLAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for {ObjectName}", objectName);
            throw;
        }
    }

    public async Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Listing buckets");
            var response = await _s3Client.ListBucketsAsync(cancellationToken);
            _logger.LogInformation("Listed buckets");
            return response.Buckets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing buckets");
            throw;
        }
    }

    public async Task<List<S3Object>> ListObjectsAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Listing objects in bucket {BucketName}", bucketName);
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            var response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
            _logger.LogInformation("Listed objects in bucket {BucketName}", bucketName);
            return response.S3Objects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing objects in bucket {BucketName}", bucketName);
            throw;
        }
    }

    public async Task<GetBucketLocationResponse> GetBucketInfoAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting info for bucket {BucketName}", bucketName);
            var request = new GetBucketLocationRequest
            {
                BucketName = bucketName
            };

            var response = await _s3Client.GetBucketLocationAsync(request, cancellationToken);
            _logger.LogInformation("Got info for bucket {BucketName}", bucketName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting info for bucket {BucketName}", bucketName);
            throw;
        }
    }

    public async Task<MetadataCollection> GetObjectMetadataAsync(string bucketName, string objectName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting metadata for object {ObjectName} in bucket {BucketName}", objectName, bucketName);
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            var response = await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            _logger.LogInformation("Got metadata for object {ObjectName} in bucket {BucketName}", objectName, bucketName);
            return response.Metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for object {ObjectName} in bucket {BucketName}", objectName, bucketName);
            throw;
        }
    }

    public async Task CopyObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Copying object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
            var request = new CopyObjectRequest
            {
                SourceBucket = sourceBucket,
                SourceKey = sourceKey,
                DestinationBucket = destinationBucket,
                DestinationKey = destinationKey
            };

            var response = await _s3Client.CopyObjectAsync(request, cancellationToken);
            _logger.LogInformation("Copied object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
            throw;
        }
    }

    public async Task MoveObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Moving object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
            await CopyObjectAsync(sourceBucket, sourceKey, destinationBucket, destinationKey, cancellationToken);
            await DeleteBlobAsync(sourceKey, cancellationToken);
            _logger.LogInformation("Moved object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving object from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", sourceBucket, sourceKey, destinationBucket, destinationKey);
            throw;
        }
    }

    public async Task CreateBucketAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating bucket {BucketName}", bucketName);
            var request = new PutBucketRequest
            {
                BucketName = bucketName
            };

            var response = await _s3Client.PutBucketAsync(request, cancellationToken);
            _logger.LogInformation("Created bucket {BucketName}", bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bucket {BucketName}", bucketName);
            throw;
        }
    }

    public async Task DeleteBucketAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting bucket {BucketName}", bucketName);
            var request = new DeleteBucketRequest
            {
                BucketName = bucketName
            };

            await _s3Client.DeleteBucketAsync(request, cancellationToken);
            _logger.LogInformation("Deleted bucket {BucketName}", bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bucket {BucketName}", bucketName);
            throw;
        }
    }
}
