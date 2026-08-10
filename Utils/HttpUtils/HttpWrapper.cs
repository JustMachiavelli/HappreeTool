using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Web;

namespace HappreeTool.Utils.HttpUtils
{
    public class HttpWrapper
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpWrapper> _logger;

        public HttpWrapper(ILogger<HttpWrapper> logger, IHttpClientFactory factory)
        {
            _client = factory.CreateClient(HttpClientExtensions.MyApiClient);
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? parameters = null)
        {
            string fullUrl = HttpCommonUtils.GetFullGetUrl(url, parameters);
            _logger.LogInformation("准备发起Http Get请求: {url}", fullUrl);

            return await _client.GetAsync(fullUrl);
        }

        public async Task<HttpResponseMessage> PostJsonAsync<T>(
            string url,
            T? content = default,
            Dictionary<string, string>? parameters = null,
            Dictionary<string, string>? headers = null,
            Dictionary<string, string>? cookies = null)
        {
            // 处理 URL 参数
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

            // 序列化 JSON 内容
            string? jsonContent = content != null
                ? JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true })
                : null;
            StringContent? httpContent = jsonContent != null
                ? new StringContent(jsonContent, Encoding.UTF8, "application/json")
                : null;

            // 创建 HttpClient
            //HttpClient client = _httpClientFactory.CreateClient("default");

            // 添加 Cookies
            if (cookies != null && cookies.Count > 0)
            {
                string cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
                _client.DefaultRequestHeaders.Add("Cookie", cookieHeader);
            }

            // 添加 Headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!_client.DefaultRequestHeaders.Contains(header.Key)) // 避免重复添加
                    {
                        _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            // 记录日志
            _logger.LogInformation(
                "准备发起 Http POST 请求: {url}，发送内容:\n{content}，Query参数: {parameters}，Headers: {headers}，Cookies: {cookies}",
                uriBuilder.ToString(),
                jsonContent,
                parameters != null ? string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")) : "无",
                headers != null ? string.Join("; ", headers.Select(h => $"{h.Key}: {h.Value}")) : "无",
                cookies != null ? string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}")) : "无");

            // 发送请求
            return await _client.PostAsync(uriBuilder.ToString(), httpContent);
        }

        public async Task<HttpResponseMessage> PostJsonAsync(
            string url,
            Dictionary<string, string>? parameters = null,
            Dictionary<string, string>? headers = null,
            Dictionary<string, string>? cookies = null)
        {
            return await PostJsonAsync<object?>(url, null, parameters, headers, cookies);
        }

        public static Dictionary<string, string> ConvertSetCookieToDictionary(IEnumerable<string> setCookieValues)
        {
            var cookieDict = new Dictionary<string, string>();

            foreach (var cookie in setCookieValues)
            {
                // 分割 cookie 内容，获取 key=value 部分
                var parts = cookie.Split(';')[0];  // 只获取 cookie 的 key=value 部分
                var keyValue = parts.Split('=');

                if (keyValue.Length == 2)
                {
                    cookieDict[keyValue[0].Trim()] = keyValue[1].Trim();  // 将键值对加入字典
                }
            }

            return cookieDict;
        }

    }
}
