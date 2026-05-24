namespace HappreeTool.ApiAbouts.Messages
{
    public enum ApiCode
    {
        服务器内部错误 = 1000,

        成功 = 2000,
        资源抓取中可以稍后重试 = 2001,
        资源已存在 = 2002,

        请求参数非法 = 4000,
    }
}
