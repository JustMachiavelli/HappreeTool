namespace HappreeTool.ExternalServices.FileStorage.Oss
{
    public class AliOssSettings
    {
        public string Region { get; set; } = string.Empty;

        public string Endpoint { get; set; } = string.Empty;

        public string AccessKeyId { get; set; } = string.Empty;

        public string AccessKeySecret { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;
    }
}