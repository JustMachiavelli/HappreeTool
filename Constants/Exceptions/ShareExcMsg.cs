namespace HappreeTool.Constants.Exceptions;

public class ShareExcMsg
{
    public static string ResourceNotExist(string resourceType, string value)
        => $"类型为【{resourceType}】的【{value}】资源还不存在";

    public static string ResourceNotExist<T>(string resourceType, params T[] values)
        => $"类型为【{resourceType}】的【{string.Join('、', values)}】资源还不存在";

    public static string ResourceAlreadyExist(string resourceType, params object[] values)
        => $"类型为【{resourceType}】的【{string.Join('、', values)}】已存在";

    public static string DownloadNetworkResource(string resource, int statusCode)
        => $"下载网络资源【{resource}】 发生错误: http状态码【{statusCode}】";

    public static string VisitUrlBut404(string url)
        => $"访问页面【{url}】是404";

    public static string IpBanned(string url, string website)
        => $"打开：{url} 网页失败，你的ip已被网站【{website}】封禁";
}