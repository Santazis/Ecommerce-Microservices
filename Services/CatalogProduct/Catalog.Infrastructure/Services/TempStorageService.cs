using Amazon.S3;
using Amazon.S3.Model;
using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Catalog.Infrastructure.Services;

public class TempStorageService : ITempStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3Settings> _s3Settings;
    private const string ImageFolder = "images";
    public TempStorageService(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings;
    }

    public async Task<string> SaveFilesToTempStorageAsync(Stream stream,string contentType,string fileName)
    {
        var key = Guid.NewGuid().ToString();;
        var putRequest = new PutObjectRequest()
        {
            Key = $"{ImageFolder}/{key}",
            BucketName = _s3Settings.Value.BucketName,
            InputStream = stream,
            ContentType = contentType,
            Metadata = { ["file-name"] = fileName }
        };
        await _s3Client.PutObjectAsync(putRequest);
        return putRequest.Key;
    }
}
