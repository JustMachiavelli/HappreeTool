namespace HappreeTool.ExternalServices.FileStorage
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(byte[] bytes, string objectKey);

        Task<string> UploadFileAsync(string filePath, string objectKey);

        Task DeleteAsync(string objectKey);

        Task<bool> ExistsAsync(string objectKey);

        Task<byte[]> DownloadAsync(string objectKey);
    }
}