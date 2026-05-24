using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Net;

namespace HappreeTool.Surfers
{
    public class HttpClientWrapper(ILogger<HttpClientWrapper> logger,
                                   IHttpClientFactory _httpClientFactory)
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions() 
        {
            WriteIndented = true,  // 启用格式化输出
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 使输出更人性化
        };

        public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T content)
        {
            string jsonContent = JsonSerializer.Serialize(content, options);
            StringContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            logger.LogInformation("准备向信任服务{url} 发送内容:\n{content}", url, jsonContent);

            HttpClient client = _httpClientFactory.CreateClient(HttpClientExtensions.MyApiClient);
            HttpResponseMessage response = await client.PostAsync(url, httpContent);
            return response;
        }

        public async Task<T?> GetJsonAsync<T>(string url)
        {
            logger.LogInformation("准备从信任服务【{url}】获取数据", url);

            HttpClient client = _httpClientFactory.CreateClient(HttpClientExtensions.MyApiClient);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                logger.LogInformation("成功从{url} 获取到数据:\n{jsonResponse}", url,
                    JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(jsonResponse), options));
                T? result = JsonSerializer.Deserialize<T>(jsonResponse);
                return result;
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                logger.LogWarning("从{url} 获取数据失败，请稍后重试，状态码: {StatusCode}", url, response.StatusCode);
                return default;
            }
            else
            {
                logger.LogWarning("从{url} 获取数据失败，状态码: {StatusCode}", url, response.StatusCode);
                return default;
            }
        }
    }
}
