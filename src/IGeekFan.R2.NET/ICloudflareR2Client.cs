using Amazon.S3.Model;

namespace IGeekFan.R2.NET;

public interface ICloudflareR2Client
{
    Task<string> UploadBlobAsync(Stream blob, string blobName, CancellationToken cancellationToken);
    Task<Stream> GetBlobAsync(string blobName, CancellationToken cancellationToken);
    Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken);
    Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken);
    Task<List<S3Object>> ListObjectsAsync(string bucketName, CancellationToken cancellationToken);
    Task<GetBucketLocationResponse> GetBucketInfoAsync(string bucketName, CancellationToken cancellationToken);
    Task<MetadataCollection> GetObjectMetadataAsync(string bucketName, string objectName, CancellationToken cancellationToken);
    Task CopyObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, CancellationToken cancellationToken);
    Task MoveObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, CancellationToken cancellationToken);
    Task CreateBucketAsync(string bucketName, CancellationToken cancellationToken);
    Task DeleteBucketAsync(string bucketName, CancellationToken cancellationToken);
}
