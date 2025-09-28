namespace ImageProcessing.Options;

public class S3Settings
{
    public string BucketName { get; set; } = string.Empty;
    public string TempBucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
}