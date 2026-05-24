using System.Text;
using System.Text.Json;

namespace HappreeTool.ApiAbouts.Formats
{
    public class JsonApiFormat
    {
        public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new JsonSerializerOptions()
        {
            WriteIndented = true,  // 启用格式化输出
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 使输出更人性化
        };

        public static StringContent FormatJsonContent<T>(T content)
        {
            string jsonContent = JsonSerializer.Serialize(content, JSON_SERIALIZER_OPTIONS);
            return new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
    }
}
