namespace HappreeTool.Utils.Documents;

public static class OssFileUtils
{
        
    public static string NormalizeOssObjectKey(string path)
    {
        return path.Replace('\\', '/')
            .TrimStart('/');
    }
}