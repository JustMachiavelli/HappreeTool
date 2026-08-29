using Microsoft.Extensions.Options;

namespace HappreeTool.ExternalServices.FileStorage.Oss;

public class AliOssFileStorage : IOssFileStorage
{
    private readonly AlibabaCloud.OSS.V2.Client _client;
    private readonly string _bucketName;

    public AliOssFileStorage(IOptions<AliOssSettings> options)
    {
        var ossSettings = options.Value;

        var cfg = new AlibabaCloud.OSS.V2.Configuration
        {
            Region = ossSettings.Region,
            Endpoint = ossSettings.Endpoint,
            CredentialsProvider = new AlibabaCloud.OSS.V2.Credentials.StaticCredentialsProvider(
                ossSettings.AccessKeyId,
                ossSettings.AccessKeySecret
            ),
            ConnectTimeout = TimeSpan.FromSeconds(30),
            ReadWriteTimeout = TimeSpan.FromMinutes(20)
        };

        _client = new AlibabaCloud.OSS.V2.Client(cfg);
        _bucketName = ossSettings.BucketName;
    }

    public async Task<string> SaveAsync(byte[] bytes, string objectKey)
    {
        using var stream = new MemoryStream(bytes);
        var request = new AlibabaCloud.OSS.V2.Models.PutObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey,
            Body = stream
        };
        await _client.PutObjectAsync(request);
        return objectKey; // 或返回完整URL，若需可拼接 endpoint + bucket + key
    }
    
    public async Task UploadFileAsync(string filePath, string objectKey)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("上传文件不存在", filePath);
        }

        await using var fileStream = File.OpenRead(filePath);
        var request = new AlibabaCloud.OSS.V2.Models.PutObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey,
            Body = fileStream
        };
        await _client.PutObjectAsync(request);
    }

    public async Task DeleteAsync(string objectKey)
    {
        var request = new AlibabaCloud.OSS.V2.Models.DeleteObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey
        };
        await _client.DeleteObjectAsync(request);
    }

    public async Task<bool> ExistsAsync(string objectKey)
    {
        var request = new AlibabaCloud.OSS.V2.Models.HeadObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey
        };

        try
        {
            await _client.HeadObjectAsync(request);
            return true;
        }
        catch (AlibabaCloud.OSS.V2.OperationException ex)
            when (ex.InnerException is AlibabaCloud.OSS.V2.ServiceException serviceException &&
                  serviceException.ErrorCode == "NoSuchKey")
        {
            return false;
        }
    }

    public async Task<byte[]> DownloadAsync(string objectKey)
    {
        var request = new AlibabaCloud.OSS.V2.Models.GetObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey
        };

        var result = await _client.GetObjectAsync(
            request,
            System.Net.Http.HttpCompletionOption.ResponseContentRead);

        await using var body = result.Body!;
        using var memoryStream = new MemoryStream();

        await body.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
    
    public string GeneratePresignedUrlAsync(string objectKey, TimeSpan expiration)
    {
        var request = new AlibabaCloud.OSS.V2.Models.GetObjectRequest
        {
            Bucket = _bucketName,
            Key = objectKey
        };

        var result = _client.Presign(request, DateTime.Now.Add(expiration));

        return result.Url!;
    }
}