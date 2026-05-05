using System.Web;

namespace HappreeTool.Utils.HttpUtils
{
    public static class HttpCommonUtils
    {
        public static string GetFullGetUrl(string url, Dictionary<string, string>? parameters = null)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query[param.Key] = param.Value;
                }
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}
